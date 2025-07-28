namespace Blog.Application.Common.Messages;

/// <summary>
/// Email bildirim mesajı
/// </summary>
public record EmailNotificationMessage
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string ToEmail { get; init; } = string.Empty;
    public string ToName { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public string? TemplateName { get; init; }
    public Dictionary<string, object>? TemplateData { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public int Priority { get; init; } = 1; // 1=Normal, 2=High, 3=Critical
} 