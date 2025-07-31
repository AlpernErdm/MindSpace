using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Authentication.DTOs;
using Blog.Application.Features.Authentication.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Application.Features.Authentication.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtTokenService jwtTokenService,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _unitOfWork.Users.IsEmailExistsAsync(request.Email))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Bu email adresi zaten kullanılıyor",
                Errors = ["Email adresi zaten mevcut"]
            };
        }

        if (await _unitOfWork.Users.IsUserNameExistsAsync(request.UserName))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Bu kullanıcı adı zaten kullanılıyor",
                Errors = ["Kullanıcı adı zaten mevcut"]
            };
        }

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            JoinDate = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Kullanıcı oluşturulamadı",
                Errors = result.Errors.Select(e => e.Description).ToList()
            };
        }

        await _userManager.AddToRoleAsync(user, "Author");

        var token = await _jwtTokenService.GenerateTokenAsync(user);
        var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Kullanıcı başarıyla oluşturuldu",
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiry = DateTime.UtcNow.AddDays(1),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        User? user = null;

        if (request.EmailOrUserName.Contains("@"))
        {
            user = await _userManager.FindByEmailAsync(request.EmailOrUserName);
        }
        else
        {
            user = await _userManager.FindByNameAsync(request.EmailOrUserName);
        }

        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Kullanıcı bulunamadı",
                Errors = ["Geçersiz kullanıcı bilgileri"]
            };
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Giriş başarısız",
                Errors = ["Geçersiz şifre"]
            };
        }

        var token = await _jwtTokenService.GenerateTokenAsync(user);
        var refreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Giriş başarılı",
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiry = DateTime.UtcNow.AddDays(1),
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token, string refreshToken)
    {
        var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token);
        if (principal == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Geçersiz token",
                Errors = ["Token doğrulanamadı"]
            };
        }

        var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Geçersiz token",
                Errors = ["Kullanıcı ID bulunamadı"]
            };
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return new AuthResponse
            {
                Success = false,
                Message = "Kullanıcı bulunamadı",
                Errors = ["Kullanıcı mevcut değil"]
            };
        }

        var newToken = await _jwtTokenService.GenerateTokenAsync(user);
        var newRefreshToken = await _jwtTokenService.GenerateRefreshTokenAsync();

        return new AuthResponse
        {
            Success = true,
            Message = "Token yenilendi",
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpiry = DateTime.UtcNow.AddDays(1),
            User = MapToUserDto(user)
        };
    }

    public async Task<bool> LogoutAsync(string userId)
    {
        await Task.Delay(1);
        return true;
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        await Task.Delay(1);
        return true;
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return null;
        }
        return MapToUserDto(user);
    }

    private static UserDto MapToUserDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Bio = user.Bio,
            ProfileImageUrl = user.ProfileImageUrl,
            IsVerified = user.IsVerified,
            FollowerCount = user.FollowerCount,
            FollowingCount = user.FollowingCount,
            JoinDate = user.JoinDate
        };
    }
} 