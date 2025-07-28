using Blog.Application.Common.Interfaces;
using Blog.Application.Common.Messages;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Services;

/// <summary>
/// Notification service implementation - Real-time bildirimler
/// </summary>
public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationHubService _hubService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IUnitOfWork unitOfWork,
        INotificationHubService hubService,
        IMessagePublisher messagePublisher,
        ILogger<NotificationService> logger)
    {
        _unitOfWork = unitOfWork;
        _hubService = hubService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task<Notification> CreateNotificationAsync(
        string userId, 
        string title, 
        string message, 
        NotificationType type, 
        string? actionUrl = null, 
        string? actionData = null, 
        string? actorId = null, 
        Guid? postId = null, 
        Guid? commentId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            ActionUrl = actionUrl,
            ActionData = actionData,
            ActorId = actorId,
            PostId = postId,
            CommentId = commentId,
            IsRead = false
        };

        await _unitOfWork.Notifications.AddAsync(notification);
        await _unitOfWork.SaveChangesAsync();

        // Real-time gönder
        await SendRealTimeNotificationAsync(userId, notification);

        _logger.LogInformation("Notification created for user {UserId}: {Title}", userId, title);

        return notification;
    }

    public async Task MarkAsReadAsync(Guid notificationId, string userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        
        if (notification != null && notification.UserId == userId)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync();

                    // Real-time update gönder
        await _hubService.SendNotificationReadToUserAsync(userId, notificationId);

            _logger.LogInformation("Notification {NotificationId} marked as read by user {UserId}", 
                notificationId, userId);
        }
    }

    public async Task MarkAllAsReadAsync(string userId)
    {
        var unreadNotifications = await _unitOfWork.Notifications.FindAsync(
            n => n.UserId == userId && !n.IsRead);

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
            _unitOfWork.Notifications.Update(notification);
        }

        await _unitOfWork.SaveChangesAsync();

        // Real-time update gönder
        await _hubService.SendAllNotificationsReadToUserAsync(userId);

        _logger.LogInformation("All notifications marked as read for user {UserId}", userId);
    }

    public async Task<(IEnumerable<Notification> notifications, int totalCount)> GetUserNotificationsAsync(
        string userId, 
        int page = 1, 
        int pageSize = 20)
    {
        var (notifications, totalCount) = await _unitOfWork.Notifications.GetPagedAsync(
            page,
            pageSize,
            n => n.UserId == userId,
            n => n.OrderByDescending(x => x.CreatedAt)
        );

        return (notifications, totalCount);
    }

    public async Task<int> GetUnreadCountAsync(string userId)
    {
        return await _unitOfWork.Notifications.CountAsync(
            n => n.UserId == userId && !n.IsRead);
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, string userId)
    {
        var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
        
        if (notification != null && notification.UserId == userId)
        {
            _unitOfWork.Notifications.Remove(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Notification {NotificationId} deleted by user {UserId}", 
                notificationId, userId);
            
            return true;
        }

        return false;
    }

    public async Task CleanupOldNotificationsAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        var oldNotifications = await _unitOfWork.Notifications.FindAsync(
            n => n.CreatedAt < cutoffDate);

        if (oldNotifications.Any())
        {
            _unitOfWork.Notifications.RemoveRange(oldNotifications);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Cleaned up {Count} old notifications", oldNotifications.Count());
        }
    }

    #region Specific Notification Types

    public async Task SendPostLikedNotificationAsync(Guid postId, string likerUserId, string postAuthorId)
    {
        // Kendi postunu beğenen kişiye bildirim gönderme
        if (likerUserId == postAuthorId) return;

        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        var liker = await _unitOfWork.Users.GetByIdAsync(likerUserId);

        if (post != null && liker != null)
        {
            // 1. Database'e kaydet ve real-time gönder
            await CreateNotificationAsync(
                userId: postAuthorId,
                title: "Post Beğenildi",
                message: $"{liker.UserName} postunuzu beğendi: {post.Title}",
                type: NotificationType.PostLiked,
                actionUrl: $"/posts/{post.Slug}",
                actorId: likerUserId,
                postId: postId
            );

            // 2. RabbitMQ'ya message gönder (email, push notifications vs. için)
            var message = new PostLikedMessage
            {
                UserId = postAuthorId,
                Title = "Post Beğenildi",
                Message = $"{liker.UserName} postunuzu beğendi: {post.Title}",
                PostId = postId,
                PostTitle = post.Title,
                PostAuthorId = postAuthorId,
                LikerUserId = likerUserId,
                LikerUserName = liker.UserName
            };

            await _messagePublisher.PublishNotificationAsync(message);
            _logger.LogInformation("🐰 PostLikedMessage published to RabbitMQ for post {PostId}", postId);
        }
    }

    public async Task SendNewCommentNotificationAsync(Guid postId, Guid commentId, string commenterUserId, string postAuthorId)
    {
        // Kendi postuna yorum yapan kişiye bildirim gönderme
        if (commenterUserId == postAuthorId) return;

        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        var commenter = await _unitOfWork.Users.GetByIdAsync(commenterUserId);

        if (post != null && commenter != null)
        {
            // 1. Database'e kaydet ve real-time gönder
            await CreateNotificationAsync(
                userId: postAuthorId,
                title: "Yeni Yorum",
                message: $"{commenter.UserName} postunuza yorum yaptı: {post.Title}",
                type: NotificationType.NewComment,
                actionUrl: $"/posts/{post.Slug}#comment-{commentId}",
                actorId: commenterUserId,
                postId: postId,
                commentId: commentId
            );

            // 2. RabbitMQ'ya message gönder
            var message = new NewCommentMessage
            {
                UserId = postAuthorId,
                Title = "Yeni Yorum",
                Message = $"{commenter.UserName} postunuza yorum yaptı: {post.Title}",
                PostId = postId,
                CommentId = commentId,
                PostTitle = post.Title,
                PostAuthorId = postAuthorId,
                CommenterUserId = commenterUserId,
                CommenterUserName = commenter.UserName,
                CommentContent = "Yorum içeriği" // TODO: Actual comment content
            };

            await _messagePublisher.PublishNotificationAsync(message);
            _logger.LogInformation("🐰 NewCommentMessage published to RabbitMQ for post {PostId}", postId);
        }
    }

    public async Task SendNewFollowerNotificationAsync(string followerId, string followedUserId)
    {
        var follower = await _unitOfWork.Users.GetByIdAsync(followerId);

        if (follower != null)
        {
            await CreateNotificationAsync(
                userId: followedUserId,
                title: "Yeni Takipçi",
                message: $"{follower.UserName} sizi takip etmeye başladı",
                type: NotificationType.NewFollower,
                actionUrl: $"/users/{follower.UserName}",
                actorId: followerId
            );
        }
    }

    public async Task SendPostPublishedNotificationAsync(Guid postId, string authorId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        var author = await _unitOfWork.Users.GetByIdAsync(authorId);

        if (post != null && author != null)
        {
            // Yazarın takipçilerini al
            var followers = await _unitOfWork.UserFollows.FindAsync(uf => uf.FollowingId == authorId);

            foreach (var follower in followers)
            {
                await CreateNotificationAsync(
                    userId: follower.FollowerId,
                    title: "Yeni Post",
                    message: $"{author.UserName} yeni bir post yayınladı: {post.Title}",
                    type: NotificationType.PostPublished,
                    actionUrl: $"/posts/{post.Slug}",
                    actorId: authorId,
                    postId: postId
                );
            }

            _logger.LogInformation("Post published notifications sent to followers of {AuthorId}", authorId);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Real-time notification gönder
    /// </summary>
    private async Task SendRealTimeNotificationAsync(string userId, Notification notification)
    {
        var notificationData = new
        {
            notification.Id,
            notification.Title,
            notification.Message,
            notification.Type,
            notification.ActionUrl,
            notification.CreatedAt,
            notification.IsRead
        };

        await _hubService.SendNotificationToUserAsync(userId, notificationData);
    }

    #endregion
} 