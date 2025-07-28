using Blog.Application.Common.Interfaces;
using Blog.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Services;

/// <summary>
/// SignalR Hub operations implementation
/// </summary>
public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ILogger<NotificationHubService> _logger;

    public NotificationHubService(
        IHubContext<NotificationHub> hubContext,
        ILogger<NotificationHubService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task SendNotificationToUserAsync(string userId, object notification)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("NewNotification", notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
        }
    }

    public async Task SendNotificationReadToUserAsync(string userId, Guid notificationId)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("NotificationRead", notificationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification read to user {UserId}", userId);
        }
    }

    public async Task SendAllNotificationsReadToUserAsync(string userId)
    {
        try
        {
            await _hubContext.Clients.Group($"User_{userId}")
                .SendAsync("AllNotificationsRead");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send all notifications read to user {UserId}", userId);
        }
    }

    public async Task SendToPostRoomAsync(Guid postId, string eventName, object data)
    {
        try
        {
            await _hubContext.Clients.Group($"Post_{postId}")
                .SendAsync(eventName, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send {EventName} to post room {PostId}", eventName, postId);
        }
    }
} 