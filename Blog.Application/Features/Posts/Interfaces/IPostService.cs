using Blog.Application.Features.Posts.DTOs;

namespace Blog.Application.Features.Posts.Interfaces;

/// <summary>
/// Post işlemleri için business logic interface
/// </summary>
public interface IPostService
{

    Task<PostResponse> CreatePostAsync(CreatePostRequest request, string authorId);
    Task<PostResponse> UpdatePostAsync(Guid postId, UpdatePostRequest request);
    Task DeletePostAsync(Guid postId);
    Task<PostResponse?> GetPostByIdAsync(Guid postId);
    Task<PostResponse?> GetPostBySlugAsync(string slug);
    Task<PagedResult<PostResponse>> GetPublishedPostsAsync(int page = 1, int pageSize = 10);
    Task<PagedResult<PostResponse>> GetUserPostsAsync(string userId, int page = 1, int pageSize = 10);
    Task<PagedResult<PostResponse>> GetPostsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10);
    Task<PagedResult<PostResponse>> GetPostsByCategorySlugAsync(string categorySlug, int page = 1, int pageSize = 10);
    Task<PagedResult<PostResponse>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10);
    Task<PagedResult<PostResponse>> SearchPostsAsync(string query, int page = 1, int pageSize = 10);
    Task<PostResponse> PublishPostAsync(Guid postId);
    Task<PostResponse> UnpublishPostAsync(Guid postId);
}
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
} 