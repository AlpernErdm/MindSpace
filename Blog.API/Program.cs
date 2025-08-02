using Blog.Infrastructure.Data;
using Blog.Domain.Entities;
using Blog.Infrastructure;
using Blog.Infrastructure.Hubs;
using Blog.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Application Services
builder.Services.AddApplication();

// Infrastructure Services (Database, Repository Pattern)
builder.Services.AddInfrastructure(builder.Configuration);

// Identity Configuration
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // Password requirements
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    
    // User requirements
    options.User.RequireUniqueEmail = true;
    
    // Sign in requirements
    options.SignIn.RequireConfirmedEmail = false; // Development için
})
.AddEntityFrameworkStores<BlogDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
    };
    
    // SignalR JWT Authentication için events
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
    };
});

// Authorization
builder.Services.AddAuthorization();

// SignalR
builder.Services.AddSignalR();

// Controllers
builder.Services.AddControllers();

// API Documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Medium Clone API", Version = "v1" });
    
    // JWT Authorization için Swagger yapılandırması
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://192.168.1.147:3000",
                "https://192.168.1.147:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medium Clone API v1"));
}

app.UseHttpsRedirection();

// Static files middleware for uploaded images
app.UseStaticFiles();

app.UseCors("AllowReactClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR Hubs
app.MapHub<NotificationHub>("/notificationHub");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

// Initialize Database with Seed Data & Elasticsearch
using (var scope = app.Services.CreateScope())
{
    try
    {
        // 1. Run Database Migrations
        var context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
        await context.Database.MigrateAsync();
        app.Logger.LogInformation("🗃️ Database migrations applied successfully");

        // 2. Seed Database
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Blog.Infrastructure.Data.SeedData.DatabaseSeeder>>();
        
        var seeder = new Blog.Infrastructure.Data.SeedData.DatabaseSeeder(context, userManager, roleManager, logger);
        await seeder.SeedAsync();

        // 3. Initialize Elasticsearch
        var elasticsearchService = scope.ServiceProvider.GetRequiredService<Blog.Application.Common.Interfaces.IElasticsearchService>();
        await elasticsearchService.InitializeAsync();
        app.Logger.LogInformation("🔍 Elasticsearch initialized successfully");

        // 4. Index Posts to Elasticsearch
        await IndexPostsToElasticsearchAsync(scope.ServiceProvider, app.Logger);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "❌ Failed to initialize database and search services");
    }
}

// Helper method for Elasticsearch indexing
static async Task IndexPostsToElasticsearchAsync(IServiceProvider serviceProvider, ILogger logger)
{
    try
    {
        var context = serviceProvider.GetRequiredService<BlogDbContext>();
        var elasticsearchService = serviceProvider.GetRequiredService<Blog.Application.Common.Interfaces.IElasticsearchService>();
        
        var posts = await context.Posts
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .Where(p => p.Status == PostStatus.Published)
            .ToListAsync();

        if (posts.Any())
        {
            var searchDocuments = posts.Select(p => new Blog.Application.Common.Search.PostSearchDocument
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                Excerpt = p.Excerpt,
                Slug = p.Slug,
                AuthorId = p.AuthorId,
                AuthorName = $"{p.Author.FirstName} {p.Author.LastName}",
                AuthorUserName = p.Author.UserName,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? "Uncategorized",
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),
                PublishedAt = p.PublishedAt ?? p.CreatedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ViewCount = p.ViewCount,
                LikeCount = 0, // Will be calculated
                CommentCount = 0, // Will be calculated
                ReadTimeMinutes = p.ReadTimeMinutes,
                Status = p.Status.ToString(),
                MetaDescription = p.MetaDescription,
                MetaKeywords = p.MetaKeywords,
                FeaturedImageUrl = p.FeaturedImageUrl
            });

            await elasticsearchService.BulkIndexPostsAsync(searchDocuments);
            logger.LogInformation("🔍 Successfully indexed {Count} posts to Elasticsearch", posts.Count);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Failed to index posts to Elasticsearch");
    }
}

app.Run();
