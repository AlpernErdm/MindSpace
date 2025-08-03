using Blog.Domain.Common;

namespace Blog.Domain.Entities;

public class Bookmark : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Post Post { get; set; } = null!;
} 