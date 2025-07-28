using Blog.Application.Features.Authentication.Interfaces;
using Blog.Application.Features.Authentication.Services;
using Blog.Application.Features.Posts.Interfaces;
using Blog.Application.Features.Posts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Application;

/// <summary>
/// Application layer için DI registration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication Services
        services.AddScoped<IAuthService, AuthService>();
        
        // Post Services
        services.AddScoped<IPostService, PostService>();

        // Auto Mapper (gelecekte eklenebilir)
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // MediatR (gelecekte CQRS için eklenebilir)
        // services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation (gelecekte eklenebilir)
        // services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
} 