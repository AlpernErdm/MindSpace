namespace Blog.Application.Common.Interfaces;

public interface INotificationHubService
{
    Task SendNotificationToUserAsync(string userId, object notification);
    Task SendNotificationReadToUserAsync(string userId, Guid notificationId);
    Task SendAllNotificationsReadToUserAsync(string userId);
    Task SendToPostRoomAsync(Guid postId, string eventName, object data);
} 