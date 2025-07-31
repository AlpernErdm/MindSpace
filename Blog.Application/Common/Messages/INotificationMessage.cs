namespace Blog.Application.Common.Messages;

public interface INotificationMessage
{
    Guid Id { get; }
    string UserId { get; }
    string Title { get; }
    string Message { get; }
    DateTime Timestamp { get; }
} 