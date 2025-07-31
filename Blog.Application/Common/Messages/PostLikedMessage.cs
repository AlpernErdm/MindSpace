namespace Blog.Application.Common.Messages;

public record PostLikedMessage : INotificationMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // Post-specific properties
    public Guid PostId { get; init; }
    public string PostTitle { get; init; } = string.Empty;
    public string PostAuthorId { get; init; } = string.Empty;
    public string LikerUserId { get; init; } = string.Empty;
    public string LikerUserName { get; init; } = string.Empty;
} 