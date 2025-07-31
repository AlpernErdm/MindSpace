using Blog.Application.Common.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(BlogDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByUserNameAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<bool> IsEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> IsUserNameExistsAsync(string userName, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(u => u.UserName == userName, cancellationToken);
    }

    public async Task<bool> IsFollowingAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserFollow>()
            .AnyAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetFollowersAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserFollow>()
            .Where(uf => uf.FollowingId == userId)
            .Include(uf => uf.Follower)
            .Select(uf => uf.Follower)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<User>> GetFollowingAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserFollow>()
            .Where(uf => uf.FollowerId == userId)
            .Include(uf => uf.Following)
            .Select(uf => uf.Following)
            .ToListAsync(cancellationToken);
    }

    public async Task FollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        // Zaten takip ediyorsa hiçbir şey yapma
        if (await IsFollowingAsync(followerId, followingId, cancellationToken))
            return;

        var userFollow = new UserFollow
        {
            FollowerId = followerId,
            FollowingId = followingId
        };

        await _context.Set<UserFollow>().AddAsync(userFollow, cancellationToken);
        
        // Follow count'ları güncelle
        await UpdateFollowCountsAsync(followerId, cancellationToken);
        await UpdateFollowCountsAsync(followingId, cancellationToken);
    }

    public async Task UnfollowUserAsync(string followerId, string followingId, CancellationToken cancellationToken = default)
    {
        var userFollow = await _context.Set<UserFollow>()
            .FirstOrDefaultAsync(uf => uf.FollowerId == followerId && uf.FollowingId == followingId, cancellationToken);

        if (userFollow != null)
        {
            _context.Set<UserFollow>().Remove(userFollow);
            
            // Follow count'ları güncelle
            await UpdateFollowCountsAsync(followerId, cancellationToken);
            await UpdateFollowCountsAsync(followingId, cancellationToken);
        }
    }

    public async Task UpdateFollowCountsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await GetByIdAsync(userId, cancellationToken);
        if (user == null) return;

        // Follower count
        user.FollowerCount = await _context.Set<UserFollow>()
            .CountAsync(uf => uf.FollowingId == userId, cancellationToken);

        // Following count
        user.FollowingCount = await _context.Set<UserFollow>()
            .CountAsync(uf => uf.FollowerId == userId, cancellationToken);

        Update(user);
    }
} 