using Blog.Application.Common.Interfaces;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Blog.Infrastructure.Repositories;
public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(BlogDbContext context) : base(context)
    {
    }

    public async Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<bool> IsSlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(p => p.Slug == slug, cancellationToken);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetPublishedPostsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p => p.Status == PostStatus.Published && p.PublishedAt != null);

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetUserPostsAsync(string userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p => p.AuthorId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByCategoryPagedAsync(Guid categoryId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p => p.CategoryId == categoryId && p.Status == PostStatus.Published);

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByCategorySlugAsync(string categorySlug, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p => p.Category.Slug == categorySlug && p.Status == PostStatus.Published);

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByTagSlugAsync(string tagSlug, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Where(p => p.Status == PostStatus.Published &&
                       p.PostTags.Any(pt => pt.Tag.Slug == tagSlug));

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.AuthorId == authorId && p.Status == PostStatus.Published)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetPostsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CategoryId == categoryId && p.Status == PostStatus.Published)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetPostsByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.PostTags.Any(pt => pt.TagId == tagId) && p.Status == PostStatus.Published)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PublishedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateViewCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await GetByIdAsync(postId, cancellationToken);
        if (post != null)
        {
            post.ViewCount++;
            Update(post);
        }
    }

    public async Task UpdateLikeCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await GetByIdAsync(postId, cancellationToken);
        if (post != null)
        {
            post.LikeCount = await _context.Set<Like>()
                .CountAsync(l => l.PostId == postId && l.Type == LikeType.Like, cancellationToken);
            
            Update(post);
        }
    }

    public async Task UpdateCommentCountAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        var post = await GetByIdAsync(postId, cancellationToken);
        if (post != null)
        {
            post.CommentCount = await _context.Set<Comment>()
                .CountAsync(c => c.PostId == postId, cancellationToken);
            
            Update(post);
        }
    }

    public async Task<IEnumerable<Post>> GetPopularPostsAsync(int count = 5, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == PostStatus.Published)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .OrderByDescending(p => p.ViewCount)
            .ThenByDescending(p => p.LikeCount)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Status == PostStatus.Published)
            .Include(p => p.Author)
            .Include(p => p.Category)
            .OrderByDescending(p => p.PublishedAt)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Post> posts, int totalCount)> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var lowercaseSearchTerm = searchTerm.ToLower();
        
        var query = _dbSet
            .Where(p => p.Status == PostStatus.Published && 
                       (p.Title.ToLower().Contains(lowercaseSearchTerm) ||
                        p.Content.ToLower().Contains(lowercaseSearchTerm) ||
                        p.Excerpt != null && p.Excerpt.ToLower().Contains(lowercaseSearchTerm)));

        var totalCount = await query.CountAsync(cancellationToken);

        var posts = await query
            .Include(p => p.Author)
            .Include(p => p.Category)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }
} 