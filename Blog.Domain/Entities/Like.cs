using Blog.Domain.Common;

namespace Blog.Domain.Entities;

public class Like : BaseEntity
{
    public LikeType Type { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
}

public enum LikeType
{
    Like = 0,
    Dislike = 1,
    Love = 2,
    Clap = 3  
} 