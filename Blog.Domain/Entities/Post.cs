using Blog.Domain.Common;

namespace Blog.Domain.Entities;

/// <summary>
/// Blog yazısı entity'si - Medium benzeri makale yapısı
/// </summary>
public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // SEO-friendly URL
    public string Content { get; set; } = string.Empty; // HTML content
    public string? Excerpt { get; set; } // Kısa özet
    public string? FeaturedImageUrl { get; set; }
    
    // Yayın durumu
    public PostStatus Status { get; set; } = PostStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    
    // İstatistikler
    public int ViewCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int CommentCount { get; set; } = 0;
    
    // Okuma süresi (dakika)
    public int ReadTimeMinutes { get; set; } = 1;
    
    // SEO
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Foreign Keys
    public string AuthorId { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    
    // Navigation Properties
    public virtual User Author { get; set; } = null!;
    public virtual Category? Category { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}

/// <summary>
/// Yazı durumu enum'u
/// </summary>
public enum PostStatus
{
    Draft = 0,      // Taslak
    Published = 1,  // Yayınlanmış
    Archived = 2    // Arşivlenmiş
} 