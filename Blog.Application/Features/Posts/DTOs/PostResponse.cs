using Blog.Domain.Entities;
using Blog.Application.Features.Authentication.DTOs;

namespace Blog.Application.Features.Posts.DTOs;

/// <summary>
/// Post response DTO'su - API'den dönen post verisi
/// </summary>
public class PostResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? Excerpt { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public PostStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ReadTimeMinutes { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Author bilgileri
    public UserDto Author { get; set; } = new UserDto();

    // Category bilgisi
    public CategoryResponse? Category { get; set; }

    // Tag listesi
    public List<TagResponse> Tags { get; set; } = new List<TagResponse>();
}

/// <summary>
/// Basit kategori bilgisi
/// </summary>
public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Color { get; set; }
}

/// <summary>
/// Basit tag bilgisi
/// </summary>
public class TagResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Color { get; set; }
} 