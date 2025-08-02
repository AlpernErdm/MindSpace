using System.ComponentModel.DataAnnotations;
using Blog.Domain.Entities;

namespace Blog.Application.Features.Posts.DTOs;


public class CreatePostRequest
{
    [Required(ErrorMessage = "Başlık gereklidir")]
    [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik gereklidir")]
    [MinLength(10, ErrorMessage = "İçerik en az 10 karakter olmalıdır")]
    public string Content { get; set; } = string.Empty;

    [MaxLength(500, ErrorMessage = "Özet en fazla 500 karakter olabilir")]
    public string? Excerpt { get; set; }

    public string? FeaturedImageUrl { get; set; }

    [Required]
    public PostStatus Status { get; set; } = PostStatus.Draft;

    public Guid? CategoryId { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    [MaxLength(160, ErrorMessage = "Meta açıklama en fazla 160 karakter olabilir")]
    public string? MetaDescription { get; set; }

    [MaxLength(100, ErrorMessage = "Meta anahtar kelimeler en fazla 100 karakter olabilir")]
    public string? MetaKeywords { get; set; }
} 