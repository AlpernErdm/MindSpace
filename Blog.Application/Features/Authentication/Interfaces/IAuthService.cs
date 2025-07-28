using Blog.Application.Features.Authentication.DTOs;

namespace Blog.Application.Features.Authentication.Interfaces;

/// <summary>
/// Authentication Service Interface
/// </summary>
public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken);
    Task<bool> LogoutAsync(string userId);
    Task<bool> RevokeTokenAsync(string token);
    Task<UserDto?> GetCurrentUserAsync(string userId);
} 