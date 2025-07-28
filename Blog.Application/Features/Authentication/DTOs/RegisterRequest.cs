using System.ComponentModel.DataAnnotations;

namespace Blog.Application.Features.Authentication.DTOs;

/// <summary>
/// Kullanıcı kayıt işlemi için DTO
/// </summary>
public class RegisterRequest
{
    [Required(ErrorMessage = "Email alanı zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
    [MinLength(3, ErrorMessage = "Kullanıcı adı en az 3 karakter olmalıdır")]
    [MaxLength(50, ErrorMessage = "Kullanıcı adı en fazla 50 karakter olmalıdır")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur")]
    [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur")]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "Ad en fazla 50 karakter olmalıdır")]
    public string? FirstName { get; set; }

    [MaxLength(50, ErrorMessage = "Soyad en fazla 50 karakter olmalıdır")]
    public string? LastName { get; set; }
} 