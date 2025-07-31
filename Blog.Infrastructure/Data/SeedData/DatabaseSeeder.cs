using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Data.SeedData;

public class DatabaseSeeder
{
    private readonly BlogDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        BlogDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            _logger.LogInformation("🌱 Starting database seeding...");

            // Seed order is important due to foreign key relationships
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedCategoriesAsync();
            await SeedTagsAsync();
            await SeedPostsAsync();
            await SeedCommentsAsync();
            await SeedLikesAsync();
            await SeedUserFollowsAsync();
            await SeedNotificationsAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Database seeding completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error occurred during database seeding");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[] { "Admin", "Author", "User" };

        foreach (var roleName in roles)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                _logger.LogInformation("👑 Created role: {RoleName}", roleName);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.AnyAsync()) return;

        var users = new[]
        {
            new { User = new User 
            { 
                UserName = "admin", 
                Email = "admin@mediumclone.com", 
                FirstName = "Admin", 
                LastName = "User",
                Bio = "System Administrator",
                EmailConfirmed = true 
            }, Password = "Admin123!", Role = "Admin" },
            
            new { User = new User 
            { 
                UserName = "john.doe", 
                Email = "john@example.com", 
                FirstName = "John", 
                LastName = "Doe",
                Bio = "Senior Software Developer. Passionate about .NET, React, and clean code.",
                EmailConfirmed = true 
            }, Password = "John123!", Role = "Author" },
            
            new { User = new User 
            { 
                UserName = "jane.smith", 
                Email = "jane@example.com", 
                FirstName = "Jane", 
                LastName = "Smith",
                Bio = "Tech Lead at Microsoft. Love sharing knowledge about cloud technologies.",
                EmailConfirmed = true 
            }, Password = "Jane123!", Role = "Author" },
            
            new { User = new User 
            { 
                UserName = "ahmet.yilmaz", 
                Email = "ahmet@example.com", 
                FirstName = "Ahmet", 
                LastName = "Yılmaz",
                Bio = "Full-stack developer from Turkey. Angular and .NET enthusiast.",
                EmailConfirmed = true 
            }, Password = "Ahmet123!", Role = "Author" },
            
            new { User = new User 
            { 
                UserName = "sarah.johnson", 
                Email = "sarah@example.com", 
                FirstName = "Sarah", 
                LastName = "Johnson",
                Bio = "UX Designer and Frontend Developer. React, Vue.js, and design systems expert.",
                EmailConfirmed = true 
            }, Password = "Sarah123!", Role = "Author" },
            
            new { User = new User 
            { 
                UserName = "mike.chen", 
                Email = "mike@example.com", 
                FirstName = "Mike", 
                LastName = "Chen",
                Bio = "DevOps Engineer. Kubernetes, Docker, and CI/CD pipeline specialist.",
                EmailConfirmed = true 
            }, Password = "Mike123!", Role = "Author" },
            
            new { User = new User 
            { 
                UserName = "test.user", 
                Email = "test@example.com", 
                FirstName = "Test", 
                LastName = "User",
                Bio = "Just a regular user testing the platform.",
                EmailConfirmed = true 
            }, Password = "Test123!", Role = "User" }
        };

        foreach (var userData in users)
        {
            var result = await _userManager.CreateAsync(userData.User, userData.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(userData.User, userData.Role);
                _logger.LogInformation("👤 Created user: {UserName} with role {Role}", userData.User.UserName, userData.Role);
            }
        }
    }

    private async Task SeedCategoriesAsync()
    {
        if (await _context.Categories.AnyAsync()) return;

        _context.Categories.AddRange(CategorySeedData.Categories);
        await _context.SaveChangesAsync();
        _logger.LogInformation("📂 Created {Count} categories", CategorySeedData.Categories.Length);
    }

    private async Task SeedTagsAsync()
    {
        if (await _context.Tags.AnyAsync()) return;

        _context.Tags.AddRange(TagSeedData.Tags);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🏷️ Created {Count} tags", TagSeedData.Tags.Length);
    }

    private async Task SeedPostsAsync()
    {
        // Mevcut postları temizle ve yenilerini ekle
        if (await _context.Posts.AnyAsync())
        {
            _logger.LogInformation("🗑️ Clearing existing posts...");
            _context.Posts.RemoveRange(await _context.Posts.ToListAsync());
            _context.PostTags.RemoveRange(await _context.PostTags.ToListAsync());
            await _context.SaveChangesAsync();
        }

        var users = await _userManager.Users.Where(u => u.UserName != "admin").ToListAsync();
        var categories = await _context.Categories.ToListAsync();
        var tags = await _context.Tags.ToListAsync();

        var posts = new List<Post>();
        var postTags = new List<PostTag>();

        for (int i = 0; i < PostSeedData.PostTemplates.Length; i++)
        {
            var template = PostSeedData.PostTemplates[i];
            var author = users[i % users.Count];
            var category = categories.FirstOrDefault(c => c.Name == template.CategoryName);
            
            if (category == null)
            {
                _logger.LogWarning("Category not found: {CategoryName}", template.CategoryName);
                continue;
            }
            
            var post = new Post
            {
                Id = Guid.NewGuid(),
                Title = template.Title,
                Content = template.Content,
                Excerpt = GenerateExcerpt(template.Content),
                Slug = GenerateSlug(template.Title),
                AuthorId = author.Id,
                CategoryId = category.Id,
                Status = PostStatus.Published,
                PublishedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(31, 60)),
                UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                ViewCount = Random.Shared.Next(50, 5000),
                ReadTimeMinutes = Random.Shared.Next(3, 15),
                MetaDescription = GenerateExcerpt(template.Content),
                MetaKeywords = string.Join(", ", template.TagNames),
                FeaturedImageUrl = $"https://picsum.photos/800/400?random={i + 1}"
            };

            posts.Add(post);

            // Add PostTags
            foreach (var tagName in template.TagNames)
            {
                var tag = tags.FirstOrDefault(t => t.Name == tagName);
                if (tag != null)
                {
                    postTags.Add(new PostTag
                    {
                        PostId = post.Id,
                        TagId = tag.Id
                    });
                }
            }
        }

        _context.Posts.AddRange(posts);
        _context.PostTags.AddRange(postTags);
        await _context.SaveChangesAsync();
        _logger.LogInformation("📝 Created {PostCount} posts with {TagCount} post-tag relationships", posts.Count, postTags.Count);
    }

    private async Task SeedCommentsAsync()
    {
        if (await _context.Comments.AnyAsync()) return;

        var posts = await _context.Posts.ToListAsync();
        var users = await _userManager.Users.ToListAsync();
        var comments = new List<Comment>();

        foreach (var post in posts.Take(10)) // Add comments to first 10 posts
        {
            // Root comments
            for (int i = 0; i < Random.Shared.Next(2, 8); i++)
            {
                var user = users[Random.Shared.Next(users.Count)];
                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    Content = GetRandomCommentContent(),
                    PostId = post.Id,
                    AuthorId = user.Id,
                    CreatedAt = post.PublishedAt?.AddDays(Random.Shared.Next(1, 10)) ?? DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                comments.Add(comment);

                // Nested replies (50% chance)
                if (Random.Shared.Next(0, 2) == 1)
                {
                    for (int j = 0; j < Random.Shared.Next(1, 3); j++)
                    {
                        var replyUser = users[Random.Shared.Next(users.Count)];
                        var reply = new Comment
                        {
                            Id = Guid.NewGuid(),
                            Content = GetRandomReplyContent(),
                            PostId = post.Id,
                            AuthorId = replyUser.Id,
                            ParentCommentId = comment.Id,
                            CreatedAt = comment.CreatedAt.AddMinutes(Random.Shared.Next(10, 1440)),
                            UpdatedAt = DateTime.UtcNow
                        };
                        comments.Add(reply);
                    }
                }
            }
        }

        _context.Comments.AddRange(comments);
        await _context.SaveChangesAsync();
        _logger.LogInformation("💬 Created {Count} comments with nested replies", comments.Count);
    }

    private async Task SeedLikesAsync()
    {
        if (await _context.Likes.AnyAsync()) return;

        var posts = await _context.Posts.ToListAsync();
        var comments = await _context.Comments.ToListAsync();
        var users = await _userManager.Users.ToListAsync();
        var likes = new List<Like>();

        // Post likes
        foreach (var post in posts)
        {
            var likeCount = Random.Shared.Next(1, 20);
            var randomUsers = users.OrderBy(x => Guid.NewGuid()).Take(likeCount);
            
            foreach (var user in randomUsers)
            {
                var likeTypes = new[] { LikeType.Like, LikeType.Love, LikeType.Clap };
                likes.Add(new Like
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PostId = post.Id,
                    Type = likeTypes[Random.Shared.Next(likeTypes.Length)],
                    CreatedAt = post.PublishedAt?.AddDays(Random.Shared.Next(1, 10)) ?? DateTime.UtcNow
                });
            }
        }

        // Comment likes
        foreach (var comment in comments.Take(50)) // First 50 comments
        {
            var likeCount = Random.Shared.Next(0, 5);
            var randomUsers = users.OrderBy(x => Guid.NewGuid()).Take(likeCount);
            
            foreach (var user in randomUsers)
            {
                likes.Add(new Like
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    CommentId = comment.Id,
                    Type = LikeType.Like,
                    CreatedAt = comment.CreatedAt.AddMinutes(Random.Shared.Next(10, 1440))
                });
            }
        }

        _context.Likes.AddRange(likes);
        await _context.SaveChangesAsync();
        _logger.LogInformation("❤️ Created {Count} likes for posts and comments", likes.Count);
    }

    private async Task SeedUserFollowsAsync()
    {
        if (await _context.UserFollows.AnyAsync()) return;

        var users = await _userManager.Users.ToListAsync();
        var follows = new List<UserFollow>();

        foreach (var user in users)
        {
            // Each user follows 2-5 random other users
            var followCount = Random.Shared.Next(2, 6);
            var usersToFollow = users.Where(u => u.Id != user.Id)
                                   .OrderBy(x => Guid.NewGuid())
                                   .Take(followCount);

            foreach (var userToFollow in usersToFollow)
            {
                follows.Add(new UserFollow
                {
                    FollowerId = user.Id,
                    FollowingId = userToFollow.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 60))
                });
            }
        }

        _context.UserFollows.AddRange(follows);
        await _context.SaveChangesAsync();
        _logger.LogInformation("👥 Created {Count} user follow relationships", follows.Count);
    }

    private async Task SeedNotificationsAsync()
    {
        if (await _context.Notifications.AnyAsync()) return;

        var users = await _userManager.Users.ToListAsync();
        var posts = await _context.Posts.ToListAsync();
        var comments = await _context.Comments.ToListAsync();
        var notifications = new List<Notification>();

        foreach (var user in users.Take(5)) // First 5 users get notifications
        {
            // Post liked notifications
            for (int i = 0; i < Random.Shared.Next(3, 8); i++)
            {
                var post = posts[Random.Shared.Next(posts.Count)];
                var actor = users[Random.Shared.Next(users.Count)];
                
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Title = "Post Beğenildi",
                    Message = $"{actor.FirstName} {actor.LastName} postunuzu beğendi: {post.Title}",
                    Type = NotificationType.PostLiked,
                    ActionUrl = $"/posts/{post.Slug}",
                    ActorId = actor.Id,
                    PostId = post.Id,
                    IsRead = Random.Shared.Next(0, 2) == 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 15))
                });
            }

            // New comment notifications
            for (int i = 0; i < Random.Shared.Next(2, 5); i++)
            {
                var comment = comments[Random.Shared.Next(comments.Count)];
                var actor = users[Random.Shared.Next(users.Count)];
                
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Title = "Yeni Yorum",
                    Message = $"{actor.FirstName} {actor.LastName} postunuza yorum yaptı",
                    Type = NotificationType.NewComment,
                    ActionUrl = $"/posts/{comment.PostId}#comment-{comment.Id}",
                    ActorId = actor.Id,
                    PostId = comment.PostId,
                    CommentId = comment.Id,
                    IsRead = Random.Shared.Next(0, 2) == 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10))
                });
            }

            // New follower notifications
            for (int i = 0; i < Random.Shared.Next(1, 4); i++)
            {
                var follower = users[Random.Shared.Next(users.Count)];
                
                notifications.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Title = "Yeni Takipçi",
                    Message = $"{follower.FirstName} {follower.LastName} sizi takip etmeye başladı",
                    Type = NotificationType.NewFollower,
                    ActionUrl = $"/users/{follower.UserName}",
                    ActorId = follower.Id,
                    IsRead = Random.Shared.Next(0, 2) == 1,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 20))
                });
            }
        }

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();
        _logger.LogInformation("🔔 Created {Count} notifications", notifications.Count);
    }

    #region Helper Methods

    private string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ç", "c")
            .Replace("ğ", "g")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ş", "s")
            .Replace("ü", "u")
            .Replace(":", "")
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "");
    }

    private string GenerateExcerpt(string content)
    {
        return content.Length > 200 ? content.Substring(0, 200) + "..." : content;
    }

    private string GetRandomCommentContent()
    {
        var comments = new[]
        {
            "Harika bir yazı olmuş! Bu konuda daha fazla içerik görmek isterim.",
            "Çok faydalı bilgiler paylaşmışsınız. Teşekkürler!",
            "Bu yaklaşımı projemde de kullanmayı düşünüyorum. Gerçekten pratik çözümler.",
            "Konuyu çok net açıklamışsınız. Yeni başlayanlar için mükemmel rehber.",
            "Deneyimlerinizi paylaştığınız için teşekkürler. Çok değerli bilgiler.",
            "Bu makaleyi okuduktan sonra konuyu çok daha iyi anladım.",
            "Kod örnekleri gerçekten yardımcı oldu. Hemen deneyeceğim.",
            "Alternatif yaklaşımları da görmek güzel. Hangi durumda hangisini kullanmalıyız?",
            "Bu teknoloji gerçekten game-changer. Projelerimde mutlaka kullanacağım.",
            "Detaylı anlatım için teşekkürler. Bookmark'ladım."
        };
        return comments[Random.Shared.Next(comments.Length)];
    }

    private string GetRandomReplyContent()
    {
        var replies = new[]
        {
            "Katılıyorum! Bu konuda benzer deneyimlerim var.",
            "Haklısınız, ben de aynı şekilde düşünüyorum.",
            "Bu noktada farklı bir yaklaşım deneyebilirsiniz.",
            "Teşekkürler, yorumunuz çok faydalı.",
            "Bu durumda X yaklaşımını öneririm.",
            "Deneyim paylaşımınız çok değerli.",
            "Bu konuda daha detaylı bilgi var mı?",
            "Alternatif çözümler de var tabii ki.",
            "Projemde benzer sorunlarla karşılaştım.",
            "Kesinlikle doğru bir tespit."
        };
        return replies[Random.Shared.Next(replies.Length)];
    }

    // Content generation methods
    private string GetDotNetContent() => @"
.NET 9, Microsoft'un en son .NET platformu sürümü olarak birçok heyecan verici özellik ve performance iyileştirmesi getiriyor. Bu yazıda, .NET 9'daki en önemli yenilikleri ve bunların geliştiriciler için nasıl avantajlar sağladığını inceleyeceğiz.

## Performance İyileştirmeleri

.NET 9, önceki sürümlere kıyasla önemli performance iyileştirmeleri içeriyor:

- **GC (Garbage Collector) İyileştirmeleri**: Yeni generation algorithm ile %20'ye varan performance artışı
- **JIT Compiler Optimizations**: Startup time'da %15 iyileşme
- **Memory Allocation**: Reduced memory footprint ve daha efficient allocation

## Yeni Özellikler

### 1. Enhanced Minimal APIs
```csharp
app.MapGet(""/products/{id:int}"", async (int id, IProductService service) => 
{
    var product = await service.GetByIdAsync(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
});
```

### 2. Native AOT İyileştirmeleri
Native AOT desteği genişletildi ve daha fazla scenario destekleniyor.

### 3. New LINQ Methods
Yeni LINQ extension methods ile daha expressive kod yazabiliyoruz.

## Migration Rehberi

.NET 8'den .NET 9'a geçiş oldukça smooth bir süreç. Breaking changes minimum seviyede tutulmuş.

Bu gelişmeler .NET ecosystem'ini daha da güçlendiriyor ve modern application development için mükemmel bir platform sunuyor.
";

    private string GetReactContent() => @"
React 18 ile birlikte gelen Concurrent Features, React uygulamalarının performance ve user experience açısından devrim niteliğinde değişiklikler getiriyor. Bu yazıda bu özellikleri detaylı olarak inceleyeceğiz.

## Concurrent Rendering Nedir?

Concurrent rendering, React'in main thread'i block etmeden rendering işlemlerini yapabilmesini sağlayan bir tekniktir. Bu sayede:

- Daha responsive user interface
- Better prioritization of updates
- Improved user experience

## Yeni Hooks

### useTransition
```jsx
import { useTransition, useState } from 'react';

function SearchBox() {
  const [isPending, startTransition] = useTransition();
  const [query, setQuery] = useState('');
  const [results, setResults] = useState([]);

  const handleChange = (e) => {
    setQuery(e.target.value);
    startTransition(() => {
      // Non-urgent update
      setResults(searchFunction(e.target.value));
    });
  };

  return (
    <div>
      <input value={query} onChange={handleChange} />
      {isPending && <div>Searching...</div>}
      <SearchResults results={results} />
    </div>
  );
}
```

### useDeferredValue
Bu hook ile expensive calculations'ı defer edebiliyoruz.

## Suspense İyileştirmeleri

React 18'de Suspense daha güçlü hale geldi ve server-side rendering ile daha iyi entegre oldu.

## Performance Best Practices

1. Use concurrent features judiciously
2. Profile your applications
3. Optimize component rendering

Bu özellikler React'i modern web development için vazgeçilmez hale getiriyor.
";

    private string GetMicroservicesContent() => @"
Microservices architecture günümüzde enterprise aplikasyonlar için standart haline geldi. Bu yazıda .NET ve Docker kullanarak microservices nasıl implement edeceğimizi göreceğiz.

## Microservices Nedir?

Microservices, büyük aplikasyonları küçük, bağımsız servisler halinde bölen bir architectural pattern'dir.

### Avantajları:
- Independent deployment
- Technology diversity
- Fault isolation
- Scalability

### Dezavantajları:
- Distributed system complexity
- Network latency
- Data consistency challenges

## .NET ile Implementation

### API Gateway Pattern
```csharp
// Ocelot configuration
{
  ""Routes"": [
    {
      ""DownstreamPathTemplate"": ""/api/products/{everything}"",
      ""DownstreamScheme"": ""http"",
      ""DownstreamHostAndPorts"": [
        {
          ""Host"": ""product-service"",
          ""Port"": 80
        }
      ],
      ""UpstreamPathTemplate"": ""/products/{everything}""
    }
  ]
}
```

### Service Discovery
Consul veya Eureka kullanarak service discovery implement edebiliriz.

### Circuit Breaker Pattern
```csharp
services.AddHttpClient<IProductService, ProductService>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());
```

## Docker ile Containerization

Her microservice kendi container'ında çalışır:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY [""ProductService.csproj"", "".""]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [""dotnet"", ""ProductService.dll""]
```

## Orchestration with Docker Compose

```yaml
version: '3.8'
services:
  api-gateway:
    build: ./ApiGateway
    ports:
      - ""5000:80""
    depends_on:
      - product-service
      - user-service
  
  product-service:
    build: ./ProductService
    environment:
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=Products;
  
  user-service:
    build: ./UserService
```

Microservices architecture ile scalable ve maintainable systems build edebiliriz.
";

    // Diğer content generation methods...
    private string GetTypeScriptContent() => "TypeScript Advanced Types content...";
    private string GetElasticsearchContent() => "Elasticsearch implementation content...";
    private string GetSignalRContent() => "SignalR real-time communication content...";
    private string GetRabbitMQContent() => "RabbitMQ messaging patterns content...";
    private string GetCleanArchContent() => "Clean Architecture principles content...";
    private string GetReactPerfContent() => "React performance optimization content...";
    private string GetDevOpsContent() => "Azure DevOps CI/CD content...";
    private string GetEFContent() => "Entity Framework Core content...";
    private string GetJWTContent() => "JWT authentication content...";
    private string GetFrameworkContent() => "Framework comparison content...";
    private string GetDockerSecContent() => "Docker security content...";
    private string GetGraphQLContent() => "GraphQL API content...";
    private string GetMobileContent() => "Mobile development content...";
    private string GetMLContent() => "Machine learning content...";
    private string GetPostgreSQLContent() => "PostgreSQL performance content...";
    private string GetK8sContent() => "Kubernetes deployment content...";
    private string GetCareerContent() => "Career development content...";

    #endregion
} 