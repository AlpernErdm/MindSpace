using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Hubs;

[Authorize] 
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            _logger.LogInformation("User {UserId} connected to NotificationHub with ConnectionId {ConnectionId}", 
                userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
        }

        if (exception != null)
        {
            _logger.LogError(exception, "User disconnected with exception");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} marked notification {NotificationId} as read", 
                userId, notificationId);
            await Clients.Group($"User_{userId}").SendAsync("NotificationRead", notificationId);
        }
    }


    public async Task JoinPostRoom(Guid postId)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Post_{postId}");
            
            _logger.LogInformation("User {UserId} joined post room {PostId}", userId, postId);
        }
    }

    public async Task LeavePostRoom(Guid postId)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Post_{postId}");
            
            _logger.LogInformation("User {UserId} left post room {PostId}", userId, postId);
        }
    }

    #region Helper Methods

    /// <summary>
    /// JWT token'dan user ID'yi al
    /// </summary>
    private string? GetCurrentUserId()
    {
        return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 