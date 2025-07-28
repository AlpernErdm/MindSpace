using Blog.Domain.Common;

namespace Blog.Domain.Entities;

/// <summary>
/// Beğeni entity'si - Post ve Comment için ortak beğeni sistemi
/// Medium benzeri clap/like özelliği
/// </summary>
public class Like : BaseEntity
{
    public LikeType Type { get; set; }
    
    // Foreign Keys
    public string UserId { get; set; } = string.Empty;
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
}

/// <summary>
/// Beğeni türü enum'u - Reaction tipi
/// </summary>
public enum LikeType
{
    Like = 0,
    Dislike = 1,
    Love = 2,
    Clap = 3  // Medium benzeri clap özelliği
} 