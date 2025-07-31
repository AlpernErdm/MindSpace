using Blog.Application.Features.Authentication.Interfaces;
using Blog.Application.Features.Authentication.Services;
using Blog.Application.Features.Posts.Interfaces;
using Blog.Application.Features.Posts.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blog.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Authentication Services
        services.AddScoped<IAuthService, AuthService>();
        
        // Post Services
        services.AddScoped<IPostService, PostService>();

        return services;
    }
} 