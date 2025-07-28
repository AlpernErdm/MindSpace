namespace Blog.Domain.Common;

/// <summary>
/// Tüm entity'lerin miras alacağı base sınıf
/// Id, CreatedAt, UpdatedAt gibi ortak alanları içerir
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
} 