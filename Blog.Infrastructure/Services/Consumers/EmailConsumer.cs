using Blog.Application.Common.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blog.Infrastructure.Services.Consumers;

/// <summary>
/// Email messages consumer - Background service
/// </summary>
public class EmailConsumer : BackgroundService
{
    private readonly ILogger<EmailConsumer> _logger;

    public EmailConsumer(ILogger<EmailConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📧 EmailConsumer started");

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
                    _logger.LogDebug("📡 EmailConsumer listening for messages...");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("🛑 EmailConsumer stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in EmailConsumer");
                await Task.Delay(1000, stoppingToken); // Wait before retry
            }
        }
    }

    /// <summary>
    /// Process EmailNotificationMessage
    /// </summary>
    public async Task HandleEmailAsync(EmailNotificationMessage message)
    {
        try
        {
            _logger.LogInformation("📧 Processing EmailNotificationMessage to: {ToEmail} with subject: {Subject}", 
                message.ToEmail, message.Subject);

            // TODO: Implement actual email sending
            // Options:
            // 1. SendGrid integration
            // 2. SMTP client
            // 3. AWS SES
            // 4. Azure Communication Services

            // Simulate email sending
            await Task.Delay(500);

            // Log successful processing
            _logger.LogInformation("✅ Email sent successfully to: {ToEmail} - MessageId: {MessageId}", 
                message.ToEmail, message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process EmailNotificationMessage: {MessageId} for {ToEmail}", 
                message.Id, message.ToEmail);
            throw;
        }
    }

    /// <summary>
    /// Process high priority emails first
    /// </summary>
    public async Task HandleHighPriorityEmailAsync(EmailNotificationMessage message)
    {
        try
        {
            _logger.LogWarning("🚨 Processing HIGH PRIORITY email to: {ToEmail} with subject: {Subject}", 
                message.ToEmail, message.Subject);

            // High priority emails should be processed immediately
            await HandleEmailAsync(message);

            _logger.LogInformation("✅ High priority email processed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process high priority email: {MessageId}", message.Id);
            throw;
        }
    }

    /// <summary>
    /// Process bulk email campaigns
    /// </summary>
    public async Task HandleBulkEmailAsync(IEnumerable<EmailNotificationMessage> messages)
    {
        try
        {
            var emailList = messages.ToList();
            _logger.LogInformation("📦 Processing bulk email batch: {Count} emails", emailList.Count);

            // Process emails in batches to avoid overwhelming email service
            const int batchSize = 10;
            var batches = emailList.Chunk(batchSize);

            foreach (var batch in batches)
            {
                var tasks = batch.Select(HandleEmailAsync);
                await Task.WhenAll(tasks);
                
                // Small delay between batches
                await Task.Delay(100);
            }

            _logger.LogInformation("✅ Bulk email batch processed successfully: {Count} emails", emailList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to process bulk email batch");
            throw;
        }
    }
} 