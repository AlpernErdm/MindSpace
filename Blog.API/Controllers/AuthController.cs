using Blog.Application.Features.Authentication.DTOs;
using Blog.Application.Features.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.API.Controllers;

/// <summary>
/// Authentication Controller - Register, Login, RefreshToken
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Kullanıcı kaydı
    /// </summary>
    /// <param name="request">Kayıt bilgileri</param>
    /// <returns>Authentication response</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(AuthResponse), 400)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Validasyon hatası",
                    Errors = errors
                });
            }

            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            _logger.LogInformation("Yeni kullanıcı kaydedildi: {UserName}", request.UserName);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Register işleminde hata: {Message}", ex.Message);
            
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Bir hata oluştu",
                Errors = ["Sunucu hatası"]
            });
        }
    }

    /// <summary>
    /// Kullanıcı girişi
    /// </summary>
    /// <param name="request">Giriş bilgileri</param>
    /// <returns>Authentication response</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(AuthResponse), 400)]
    [ProducesResponseType(typeof(AuthResponse), 401)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Validasyon hatası",
                    Errors = errors
                });
            }

            var result = await _authService.LoginAsync(request);

            if (!result.Success)
            {
                return Unauthorized(result);
            }

            _logger.LogInformation("Kullanıcı giriş yaptı: {EmailOrUserName}", request.EmailOrUserName);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login işleminde hata: {Message}", ex.Message);
            
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Bir hata oluştu",
                Errors = ["Sunucu hatası"]
            });
        }
    }

    /// <summary>
    /// Token yenileme
    /// </summary>
    /// <param name="token">Eski token</param>
    /// <param name="refreshToken">Refresh token</param>
    /// <returns>Yeni token</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), 200)]
    [ProducesResponseType(typeof(AuthResponse), 400)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromQuery] string token, [FromQuery] string refreshToken)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new AuthResponse
                {
                    Success = false,
                    Message = "Token ve refresh token gerekli",
                    Errors = ["Eksik parametreler"]
                });
            }

            var result = await _authService.RefreshTokenAsync(token, refreshToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            _logger.LogInformation("Token yenilendi");
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token yenileme işleminde hata: {Message}", ex.Message);
            
            return BadRequest(new AuthResponse
            {
                Success = false,
                Message = "Bir hata oluştu",
                Errors = ["Sunucu hatası"]
            });
        }
    }

    /// <summary>
    /// Kullanıcı çıkışı
    /// </summary>
    /// <returns>Başarı durumu</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> Logout()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await _authService.LogoutAsync(userId);
                _logger.LogInformation("Kullanıcı çıkış yaptı: {UserId}", userId);
            }

            return Ok(new { success = true, message = "Başarıyla çıkış yapıldı" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout işleminde hata: {Message}", ex.Message);
            
            return BadRequest(new { success = false, message = "Bir hata oluştu" });
        }
    }

    /// <summary>
    /// Mevcut kullanıcı bilgilerini getir
    /// </summary>
    /// <returns>Kullanıcı bilgileri</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(401)]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Veritabanından kullanıcı bilgilerini çek
            var user = await _authService.GetCurrentUserAsync(userId);
            
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetCurrentUser işleminde hata: {Message}", ex.Message);
            return BadRequest(new { success = false, message = "Bir hata oluştu" });
        }
    }
} 