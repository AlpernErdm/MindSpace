using Blog.Domain.Entities;

namespace Blog.Application.Common.Interfaces;


public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsUserNameExistsAsync(string userName, CancellationToken cancellationToken = default);
    
    // Follow operations
    Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetFollowersAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetFollowingAsync(string userId, CancellationToken cancellationToken = default);
    Task FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    Task UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default);
    
    // Statistics
    Task UpdateFollowCountsAsync(string userId, CancellationToken cancellationToken = default);
} 