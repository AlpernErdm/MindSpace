using Blog.Application.Features.Notifications.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var (notifications, totalCount) = await _notificationService
                .GetUserNotificationsAsync(userId, page, pageSize);

            var response = new
            {
                Notifications = notifications.Select(n => new
                {
                    n.Id,
                    n.Title,
                    n.Message,
                    n.Type,
                    n.ActionUrl,
                    n.IsRead,
                    n.CreatedAt,
                    n.ReadAt
                }),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting notifications for user");
            return StatusCode(500, new { Message = "Bildirimler alınırken hata oluştu" });
        }
    }

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(int), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetUnreadCount()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { UnreadCount = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting unread count for user");
            return StatusCode(500, new { Message = "Okunmamış bildirim sayısı alınırken hata oluştu" });
        }
    }
    [HttpPut("{id}/mark-read")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { Message = "Bildirim okundu olarak işaretlendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking notification {NotificationId} as read", id);
            return StatusCode(500, new { Message = "Bildirim güncellenirken hata oluştu" });
        }
    }

    [HttpPut("mark-all-read")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { Message = "Tüm bildirimler okundu olarak işaretlendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking all notifications as read for user");
            return StatusCode(500, new { Message = "Bildirimler güncellenirken hata oluştu" });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteNotification(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _notificationService.DeleteNotificationAsync(id, userId);
            if (!success)
                return NotFound(new { Message = "Bildirim bulunamadı" });

            return Ok(new { Message = "Bildirim silindi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting notification {NotificationId}", id);
            return StatusCode(500, new { Message = "Bildirim silinirken hata oluştu" });
        }
    }

    [HttpPost("test")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> CreateTestNotification()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var notification = await _notificationService.CreateNotificationAsync(
                userId: userId,
                title: "Test Bildirimi",
                message: "Bu bir test bildirimidir. SignalR real-time çalışıyor! 🎉",
                type: Domain.Entities.NotificationType.NewComment,
                actionUrl: "/test"
            );

            return Ok(new { 
                Message = "Test bildirimi oluşturuldu ve real-time gönderildi!",
                NotificationId = notification.Id 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating test notification");
            return StatusCode(500, new { Message = "Test bildirimi oluşturulurken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 