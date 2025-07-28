namespace Blog.Domain.Entities;

/// <summary>
/// User Follow Junction Table - Many-to-Many User takip sistemi
/// </summary>
public class UserFollow
{
    public string FollowerId { get; set; } = string.Empty;
    public string FollowingId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
} 