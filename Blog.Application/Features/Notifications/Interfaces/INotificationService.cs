using Blog.Domain.Entities;

namespace Blog.Application.Features.Notifications.Interfaces;

public interface INotificationService
{

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

    Task MarkAsReadAsync(Guid notificationId, string userId);
    Task MarkAllAsReadAsync(string userId);
    Task<(IEnumerable<Notification> notifications, int totalCount)> GetUserNotificationsAsync(
        string userId, 
        int page = 1, 
        int pageSize = 20);
    Task<int> GetUnreadCountAsync(string userId);
    Task<bool> DeleteNotificationAsync(Guid notificationId, string userId);
    Task CleanupOldNotificationsAsync();
    Task SendPostLikedNotificationAsync(Guid postId, string likerUserId, string postAuthorId);
    Task SendNewCommentNotificationAsync(Guid postId, Guid commentId, string commenterUserId, string postAuthorId);
    Task SendNewFollowerNotificationAsync(string followerId, string followedUserId);
    Task SendPostPublishedNotificationAsync(Guid postId, string authorId);
} 