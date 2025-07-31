using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Repositories;
using Blog.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Context
        services.AddDbContext<BlogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repository Pattern
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<INotificationHubService, NotificationHubService>();
        
        // RabbitMQ Services
        services.AddScoped<IMessagePublisher, RabbitMqMessagePublisher>();
        
        // Elasticsearch Services
        services.AddScoped<IElasticsearchService, ElasticsearchService>();
        
        // Background Services (Consumers)
        services.AddHostedService<Services.Consumers.NotificationConsumer>();
        services.AddHostedService<Services.Consumers.EmailConsumer>();

        return services;
    }
} 