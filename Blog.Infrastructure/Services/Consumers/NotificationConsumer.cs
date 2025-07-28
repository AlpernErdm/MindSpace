using Blog.Application.Common.Interfaces;
using Blog.Application.Common.Messages;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Services.Consumers;

/// <summary>
/// Notification messages consumer - Background service
/// </summary>
public class NotificationConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationConsumer> _logger;

    public NotificationConsumer(
        IServiceProvider serviceProvider, 
        ILogger<NotificationConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("🎯 NotificationConsumer started");

        // Simulating message consumption from RabbitMQ
        // TODO: MassTransit actual consumer implementation
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Simulate waiting for messages
                await Task.Delay(5000, stoppingToken);
                
                // Log that we're actively listening
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("📡 NotificationConsumer listening for messages...");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("🛑 NotificationConsumer stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in NotificationConsumer");
                await Task.Delay(1000, stoppingToken); // Wait before retry
            }
        }
    }

    /// <summary>
    /// Process PostLikedMessage
    /// </summary>
    public async Task HandlePostLikedAsync(PostLikedMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var hubService = scope.ServiceProvider.GetRequiredService<INotificationHubService>();

            _logger.LogInformation("📨 Processing PostLikedMessage: {PostId} liked by {LikerUserId}", 
                message.PostId, message.LikerUserId);

            // Create in-app notification
            await notificationService.CreateNotificationAsync(
                userId: message.PostAuthorId,
                title: "Post Beğenildi",
                message: $"{message.LikerUserName} postunuzu beğendi: {message.PostTitle}",
                type: NotificationType.PostLiked,
                actionUrl: $"/posts/{message.PostId}",
                actorId: message.LikerUserId,
                postId: message.PostId
            );

            _logger.LogInformation("✅ PostLikedMessage processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process PostLikedMessage: {MessageId}", message.Id);
            throw;
        }
    }

    /// <summary>
    /// Process NewCommentMessage
    /// </summary>
    public async Task HandleNewCommentAsync(NewCommentMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            _logger.LogInformation("📨 Processing NewCommentMessage: Comment {CommentId} on post {PostId}", 
                message.CommentId, message.PostId);

            // Create in-app notification
            await notificationService.CreateNotificationAsync(
                userId: message.PostAuthorId,
                title: "Yeni Yorum",
                message: $"{message.CommenterUserName} postunuza yorum yaptı: {message.PostTitle}",
                type: NotificationType.NewComment,
                actionUrl: $"/posts/{message.PostId}#comment-{message.CommentId}",
                actorId: message.CommenterUserId,
                postId: message.PostId,
                commentId: message.CommentId
            );

            _logger.LogInformation("✅ NewCommentMessage processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process NewCommentMessage: {MessageId}", message.Id);
            throw;
        }
    }

    /// <summary>
    /// Process PostPublishedMessage
    /// </summary>
    public async Task HandlePostPublishedAsync(PostPublishedMessage message)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            _logger.LogInformation("📨 Processing PostPublishedMessage: Post {PostId} by {AuthorName}", 
                message.PostId, message.AuthorName);

            // Send notifications to all followers
            foreach (var followerId in message.FollowerIds)
            {
                await notificationService.CreateNotificationAsync(
                    userId: followerId,
                    title: "Yeni Post",
                    message: $"{message.AuthorName} yeni bir post yayınladı: {message.PostTitle}",
                    type: NotificationType.PostPublished,
                    actionUrl: $"/posts/{message.PostSlug}",
                    actorId: message.AuthorId,
                    postId: message.PostId
                );
            }

            _logger.LogInformation("✅ PostPublishedMessage processed successfully for {FollowerCount} followers", 
                message.FollowerIds.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process PostPublishedMessage: {MessageId}", message.Id);
            throw;
        }
    }
} 