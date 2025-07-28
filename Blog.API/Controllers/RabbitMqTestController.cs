using Blog.Application.Common.Interfaces;
using Blog.Application.Common.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

/// <summary>
/// RabbitMQ Test Controller - Message queue test endpoints (Admin only)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
[Produces("application/json")]
public class RabbitMqTestController : ControllerBase
{
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<RabbitMqTestController> _logger;

    public RabbitMqTestController(
        IMessagePublisher messagePublisher,
        ILogger<RabbitMqTestController> logger)
    {
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    /// <summary>
    /// Test post liked message
    /// </summary>
    [HttpPost("test-post-liked")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> TestPostLikedMessage()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = new PostLikedMessage
            {
                UserId = userId,
                Title = "Test Post Beğenildi",
                Message = "Test kullanıcısı test postunuzu beğendi!",
                PostId = Guid.NewGuid(),
                PostTitle = "Test Post Başlığı",
                PostAuthorId = userId,
                LikerUserId = "test-liker-id",
                LikerUserName = "TestLiker"
            };

            await _messagePublisher.PublishNotificationAsync(message);

            return Ok(new 
            { 
                Message = "🐰 PostLikedMessage published to RabbitMQ successfully!",
                MessageId = message.Id,
                Type = "PostLikedMessage"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing post liked message");
            return StatusCode(500, new { Message = "Test mesajı gönderilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Test new comment message
    /// </summary>
    [HttpPost("test-new-comment")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> TestNewCommentMessage()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = new NewCommentMessage
            {
                UserId = userId,
                Title = "Test Yeni Yorum",
                Message = "Test kullanıcısı test postunuza yorum yaptı!",
                PostId = Guid.NewGuid(),
                CommentId = Guid.NewGuid(),
                PostTitle = "Test Post Başlığı",
                PostAuthorId = userId,
                CommenterUserId = "test-commenter-id",
                CommenterUserName = "TestCommenter",
                CommentContent = "Bu bir test yorumudur!"
            };

            await _messagePublisher.PublishNotificationAsync(message);

            return Ok(new 
            { 
                Message = "🐰 NewCommentMessage published to RabbitMQ successfully!",
                MessageId = message.Id,
                Type = "NewCommentMessage"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing new comment message");
            return StatusCode(500, new { Message = "Test mesajı gönderilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Test email message
    /// </summary>
    [HttpPost("test-email")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> TestEmailMessage()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = new EmailNotificationMessage
            {
                ToEmail = "test@example.com",
                ToName = "Test User",
                Subject = "Test Email from RabbitMQ",
                Body = "Bu bir test email mesajıdır. RabbitMQ queue sistemi çalışıyor! 🎉",
                Priority = 1
            };

            await _messagePublisher.PublishEmailAsync(message);

            return Ok(new 
            { 
                Message = "📧 EmailNotificationMessage published to RabbitMQ successfully!",
                MessageId = message.Id,
                Type = "EmailNotificationMessage",
                ToEmail = message.ToEmail
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing email message");
            return StatusCode(500, new { Message = "Test email mesajı gönderilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Test post published message
    /// </summary>
    [HttpPost("test-post-published")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> TestPostPublishedMessage()
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var message = new PostPublishedMessage
            {
                UserId = userId,
                Title = "Test Yeni Post",
                Message = "TestAuthor yeni bir post yayınladı!",
                PostId = Guid.NewGuid(),
                PostTitle = "Test Post Başlığı",
                PostSlug = "test-post-basligi",
                AuthorId = userId,
                AuthorName = "TestAuthor",
                PostExcerpt = "Bu bir test post özeti...",
                FollowerIds = new List<string> { "follower1", "follower2", "follower3" }
            };

            await _messagePublisher.PublishNotificationAsync(message);

            return Ok(new 
            { 
                Message = "🐰 PostPublishedMessage published to RabbitMQ successfully!",
                MessageId = message.Id,
                Type = "PostPublishedMessage",
                FollowerCount = message.FollowerIds.Count
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing post published message");
            return StatusCode(500, new { Message = "Test mesajı gönderilirken hata oluştu" });
        }
    }

    /// <summary>
    /// Test bulk messages
    /// </summary>
    [HttpPost("test-bulk-messages")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> TestBulkMessages([FromQuery] int count = 5)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var messages = new List<EmailNotificationMessage>();
            
            for (int i = 1; i <= count; i++)
            {
                messages.Add(new EmailNotificationMessage
                {
                    ToEmail = $"test{i}@example.com",
                    ToName = $"Test User {i}",
                    Subject = $"Bulk Test Email #{i}",
                    Body = $"Bu bulk test email #{i}. RabbitMQ batch processing test! 🚀",
                    Priority = 1
                });
            }

            await _messagePublisher.PublishBatchAsync(messages);

            return Ok(new 
            { 
                Message = $"📦 {count} bulk messages published to RabbitMQ successfully!",
                Count = count,
                Type = "BulkEmailNotificationMessage"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing bulk messages");
            return StatusCode(500, new { Message = "Bulk test mesajları gönderilirken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 