using Blog.Application.Common.Messages;

namespace Blog.Application.Common.Interfaces;


public interface IMessagePublisher
{
    Task PublishNotificationAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : class, INotificationMessage;
    Task PublishEmailAsync(EmailNotificationMessage message, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) 
        where T : class;
    Task PublishBatchAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken = default) 
        where T : class;
} 