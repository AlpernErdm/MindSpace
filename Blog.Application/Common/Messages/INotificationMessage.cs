namespace Blog.Application.Common.Messages;

/// <summary>
/// Base interface for all notification messages
/// </summary>
public interface INotificationMessage
{
    Guid Id { get; }
    string UserId { get; }
    string Title { get; }
    string Message { get; }
    DateTime Timestamp { get; }
} 