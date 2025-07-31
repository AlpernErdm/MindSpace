using Blog.Domain.Common;

namespace Blog.Domain.Entities;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    
    // İstatistikler
    public int LikeCount { get; set; } = 0;
    
    // Foreign Keys
    public Guid PostId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    
    // Navigation Properties
    public virtual Post Post { get; set; } = null!;
    public virtual User Author { get; set; } = null!;
    public virtual Comment? ParentComment { get; set; }
    public virtual ICollection<Comment> Replies { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
} 