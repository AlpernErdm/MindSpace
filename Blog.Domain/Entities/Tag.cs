using Blog.Domain.Common;

namespace Blog.Domain.Entities;

/// <summary>
/// Etiket entity'si - Medium benzeri tag sistemi
/// </summary>
public class Tag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // SEO-friendly URL
    public string? Description { get; set; }
    public string? Color { get; set; } // Hex color code
    
    // İstatistikler
    public int PostCount { get; set; } = 0;
    public int FollowerCount { get; set; } = 0;
    
    // Navigation Properties
    public virtual ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
} 