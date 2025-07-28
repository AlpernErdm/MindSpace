using Blog.Domain.Entities;
using System.Security.Claims;

namespace Blog.Application.Common.Interfaces;
public interface IJwtTokenService
{
    Task<string> GenerateTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<bool> ValidateTokenAsync(string token);
} 