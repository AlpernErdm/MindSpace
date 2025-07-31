using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Features.Authentication.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "Email veya kullanıcı adı zorunludur")]
    public string EmailOrUserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; } = false;
} 