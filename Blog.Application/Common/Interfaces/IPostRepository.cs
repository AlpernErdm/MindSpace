using Blog.Domain.Entities;

namespace Blog.Application.Common.Interfaces;

/// <summary>
/// Post repository - Blog yazıları için özel operasyonlar
/// </summary>
public interface IPostRepository : IRepository<Post>
{
    Task<Post?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> IsSlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    
    // Published posts with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> GetPublishedPostsAsync(int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    
    // User posts with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> GetUserPostsAsync(string userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    
    // Category posts with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByCategoryPagedAsync(Guid categoryId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    
    // Category posts by slug with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByCategorySlugAsync(string categorySlug, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    
    // Tag posts with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> GetPostsByTagSlugAsync(string tagSlug, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<Post>> GetPostsByAuthorAsync(string authorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPostsByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPostsByTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    
    // Statistics
    Task UpdateViewCountAsync(Guid postId, CancellationToken cancellationToken = default);
    Task UpdateLikeCountAsync(Guid postId, CancellationToken cancellationToken = default);
    Task UpdateCommentCountAsync(Guid postId, CancellationToken cancellationToken = default);
    
    // Featured/Popular
    Task<IEnumerable<Post>> GetPopularPostsAsync(int count = 5, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetRecentPostsAsync(int count = 10, CancellationToken cancellationToken = default);
    
    // Search with pagination (tuple: posts, totalCount)
    Task<(IEnumerable<Post> posts, int totalCount)> SearchPostsAsync(string searchTerm, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
} 