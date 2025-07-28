using Blog.Domain.Entities;

namespace Blog.Application.Features.Notifications.Interfaces;

/// <summary>
/// Notification işlemleri için business logic interface
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Yeni bildirim oluştur ve real-time gönder
    /// </summary>
    Task<Notification> CreateNotificationAsync(
        string userId, 
        string title, 
        string message, 
        NotificationType type, 
        string? actionUrl = null, 
        string? actionData = null,
        string? actorId = null,
        Guid? postId = null,
        Guid? commentId = null);

    /// <summary>
    /// Bildirimi okundu olarak işaretle
    /// </summary>
    Task MarkAsReadAsync(Guid notificationId, string userId);

    /// <summary>
    /// Kullanıcının tüm bildirimlerini okundu olarak işaretle
    /// </summary>
    Task MarkAllAsReadAsync(string userId);

    /// <summary>
    /// Kullanıcının bildirimlerini getir (sayfalı)
    /// </summary>
    Task<(IEnumerable<Notification> notifications, int totalCount)> GetUserNotificationsAsync(
        string userId, 
        int page = 1, 
        int pageSize = 20);

    /// <summary>
    /// Kullanıcının okunmamış bildirim sayısını getir
    /// </summary>
    Task<int> GetUnreadCountAsync(string userId);

    /// <summary>
    /// Bildirim sil
    /// </summary>
    Task<bool> DeleteNotificationAsync(Guid notificationId, string userId);

    /// <summary>
    /// Eski bildirimleri temizle (30 günden eski)
    /// </summary>
    Task CleanupOldNotificationsAsync();

    // Real-time notification methods
    /// <summary>
    /// Yeni post beğenildi bildirimi
    /// </summary>
    Task SendPostLikedNotificationAsync(Guid postId, string likerUserId, string postAuthorId);

    /// <summary>
    /// Yeni comment eklendi bildirimi
    /// </summary>
    Task SendNewCommentNotificationAsync(Guid postId, Guid commentId, string commenterUserId, string postAuthorId);

    /// <summary>
    /// Yeni follower bildirimi
    /// </summary>
    Task SendNewFollowerNotificationAsync(string followerId, string followedUserId);

    /// <summary>
    /// Post yayınlandı bildirimi (followers'a)
    /// </summary>
    Task SendPostPublishedNotificationAsync(Guid postId, string authorId);
} 