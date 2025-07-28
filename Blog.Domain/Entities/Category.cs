using Blog.Domain.Common;

namespace Blog.Domain.Entities;

/// <summary>
/// Kategori entity'si - Yazıları gruplamak için
/// </summary>
public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconUrl { get; set; }
    public string? Color { get; set; } // Hex color code
    
    // Hiyerarşi için
    public Guid? ParentCategoryId { get; set; }
    
    // İstatistikler
    public int PostCount { get; set; } = 0;
    
    // Navigation Properties
    public virtual Category? ParentCategory { get; set; }
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
} 