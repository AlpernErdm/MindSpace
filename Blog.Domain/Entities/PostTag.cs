namespace Blog.Domain.Entities;

/// <summary>
/// Post-Tag many-to-many ilişkisi için junction table
/// </summary>
public class PostTag
{
    public Guid PostId { get; set; }
    public Guid TagId { get; set; }
    
    // Navigation Properties
    public virtual Post Post { get; set; } = null!;
    public virtual Tag Tag { get; set; } = null!;
} 