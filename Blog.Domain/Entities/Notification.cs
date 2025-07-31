using Blog.Domain.Common;

namespace Blog.Domain.Entities;

public class Notification : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }
    
    public string? ActionUrl { get; set; } // Notification'a tıklayınca gidilecek URL
    public string? ActionData { get; set; } // JSON formatında ek data
    public string UserId { get; set; } = string.Empty; // Bildirimi alacak kullanıcı
    public string? ActorId { get; set; } // Bildirimi tetikleyen kullanıcı
    public Guid? PostId { get; set; }
    public Guid? CommentId { get; set; }
    
    // Navigation Properties
    public virtual User User { get; set; } = null!;
    public virtual User? Actor { get; set; }
    public virtual Post? Post { get; set; }
    public virtual Comment? Comment { get; set; }
}

/// <summary>
/// Bildirim türü enum'u
/// </summary>
public enum NotificationType
{
    NewComment = 0,     // Yazına yeni yorum
    NewLike = 1,        // Yazın beğenildi
    PostLiked = 2,      // Post beğenildi (alias for NewLike)
    NewFollower = 3,    // Yeni takipçi
    CommentLike = 4,    // Yorumun beğenildi
    CommentReply = 5,   // Yorumuna cevap
    PostPublished = 6   // Takip ettiğin kişi yazı yayınladı
} 