using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Hubs;

/// <summary>
/// Real-time bildirimler için SignalR Hub - Infrastructure layer'da
/// </summary>
[Authorize] // JWT authentication gerekli
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Client connection olduğunda
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            // User'ı kendi grubuna ekle (personal notifications için)
            await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
            
            _logger.LogInformation("User {UserId} connected to NotificationHub with ConnectionId {ConnectionId}", 
                userId, Context.ConnectionId);
        }

        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Client disconnect olduğunda
    /// </summary>
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

    /// <summary>
    /// Client'tan gelen "mark as read" işlemi
    /// </summary>
    public async Task MarkNotificationAsRead(Guid notificationId)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            _logger.LogInformation("User {UserId} marked notification {NotificationId} as read", 
                userId, notificationId);
            
            // Burada NotificationService çağrılabilir
            // await _notificationService.MarkAsReadAsync(notificationId, userId);
            
            // Diğer client'lara bildir (eğer gerekiyorsa)
            await Clients.Group($"User_{userId}").SendAsync("NotificationRead", notificationId);
        }
    }

    /// <summary>
    /// Client'tan gelen "join room" işlemi (örnek: post comments için)
    /// </summary>
    public async Task JoinPostRoom(Guid postId)
    {
        var userId = GetCurrentUserId();
        
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Post_{postId}");
            
            _logger.LogInformation("User {UserId} joined post room {PostId}", userId, postId);
        }
    }

    /// <summary>
    /// Client'tan gelen "leave room" işlemi
    /// </summary>
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