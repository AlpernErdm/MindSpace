namespace Blog.Application.Common.Messages;

public record PostPublishedMessage : INotificationMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string UserId { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    
    // Post-specific properties
    public Guid PostId { get; init; }
    public string PostTitle { get; init; } = string.Empty;
    public string PostSlug { get; init; } = string.Empty;
    public string AuthorId { get; init; } = string.Empty;
    public string AuthorName { get; init; } = string.Empty;
    public string PostExcerpt { get; init; } = string.Empty;
    public List<string> FollowerIds { get; init; } = new();
} 