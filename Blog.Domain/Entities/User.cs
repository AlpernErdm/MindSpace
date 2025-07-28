using Microsoft.AspNetCore.Identity;

namespace Blog.Domain.Entities;

/// <summary>
/// Kullanıcı entity'si - ASP.NET Identity ile entegre
/// Medium benzeri kullanıcı profil bilgileri içerir
/// </summary>
public class User : IdentityUser
{
    // Profil bilgileri
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public string? Website { get; set; }
    
    // Sosyal medya
    public string? TwitterHandle { get; set; }
    public string? LinkedInUrl { get; set; }
    
    // Medium benzeri özellikler
    public DateTime JoinDate { get; set; } = DateTime.UtcNow;
    public int FollowerCount { get; set; } = 0;
    public int FollowingCount { get; set; } = 0;
    public bool IsVerified { get; set; } = false;
    
    // Navigation Properties
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    
    // Following/Follower relationships through UserFollow junction table
    public virtual ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
} 