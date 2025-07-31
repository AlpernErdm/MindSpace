namespace Blog.Application.Common.Messages;

public record NewCommentMessage : INotificationMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // Comment-specific properties
    public Guid PostId { get; init; }
    public Guid CommentId { get; init; }
    public string PostTitle { get; init; } = string.Empty;
    public string PostAuthorId { get; init; } = string.Empty;
    public string CommenterUserId { get; init; } = string.Empty;
    public string CommenterUserName { get; init; } = string.Empty;
    public string CommentContent { get; init; } = string.Empty;
} 