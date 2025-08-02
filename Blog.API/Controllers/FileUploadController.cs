using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FileUploadController : ControllerBase
{
    private readonly ILogger<FileUploadController> _logger;
    private readonly IWebHostEnvironment _environment;

    public FileUploadController(ILogger<FileUploadController> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    [HttpPost("upload-image")]
    // [Authorize(Roles = "Author,Admin")] // Geçici olarak kaldırıldı
    [ProducesResponseType(typeof(FileUploadResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Dosya seçilmedi" });
            }

            // Dosya boyutu kontrolü (5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { Message = "Dosya boyutu 5MB'dan büyük olamaz" });
            }

            // Dosya tipi kontrolü
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { Message = "Sadece resim dosyaları yüklenebilir (jpg, jpeg, png, gif, webp)" });
            }

            // Uploads klasörünü oluştur
            var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Benzersiz dosya adı oluştur
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsPath, fileName);

            // Dosyayı kaydet
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // URL oluştur - Backend URL'sini kullan
            var fileUrl = $"https://localhost:7237/uploads/{fileName}";

            _logger.LogInformation("📸 Image uploaded: {FileName} by user {UserId}", fileName, GetCurrentUserId());

            return Ok(new FileUploadResponse
            {
                Success = true,
                FileUrl = fileUrl,
                FileName = fileName,
                FileSize = file.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            return StatusCode(500, new { Message = "Dosya yüklenirken hata oluştu" });
        }
    }

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

public class FileUploadResponse
{
    public bool Success { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
} 