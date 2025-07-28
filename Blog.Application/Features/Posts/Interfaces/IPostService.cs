using Blog.Application.Features.Posts.DTOs;

namespace Blog.Application.Features.Posts.Interfaces;

/// <summary>
/// Post işlemleri için business logic interface
/// </summary>
public interface IPostService
{
    /// <summary>
    /// Yeni post oluştur
    /// </summary>
    Task<PostResponse> CreatePostAsync(CreatePostRequest request, string authorId);

    /// <summary>
    /// Post güncelle
    /// </summary>
    Task<PostResponse> UpdatePostAsync(Guid postId, UpdatePostRequest request);

    /// <summary>
    /// Post sil
    /// </summary>
    Task DeletePostAsync(Guid postId);

    /// <summary>
    /// Post detayını getir (ID ile)
    /// </summary>
    Task<PostResponse?> GetPostByIdAsync(Guid postId);

    /// <summary>
    /// Post detayını getir (Slug ile) + view count artır
    /// </summary>
    Task<PostResponse?> GetPostBySlugAsync(string slug);

    /// <summary>
    /// Sayfalı post listesi (published olanlar)
    /// </summary>
    Task<PagedResult<PostResponse>> GetPublishedPostsAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Kullanıcının kendi postları (tüm status'ler)
    /// </summary>
    Task<PagedResult<PostResponse>> GetUserPostsAsync(string userId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Kategori'ye göre postlar (ID ile)
    /// </summary>
    Task<PagedResult<PostResponse>> GetPostsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10);

    /// <summary>
    /// Kategori'ye göre postlar (Slug ile)
    /// </summary>
    Task<PagedResult<PostResponse>> GetPostsByCategorySlugAsync(string categorySlug, int page = 1, int pageSize = 10);

    /// <summary>
    /// Tag'e göre postlar
    /// </summary>
    Task<PagedResult<PostResponse>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10);

    /// <summary>
    /// Arama (title, content, tags)
    /// </summary>
    Task<PagedResult<PostResponse>> SearchPostsAsync(string query, int page = 1, int pageSize = 10);

    /// <summary>
    /// Post yayınla (status = Published)
    /// </summary>
    Task<PostResponse> PublishPostAsync(Guid postId);

    /// <summary>
    /// Post'u draft'a çevir
    /// </summary>
    Task<PostResponse> UnpublishPostAsync(Guid postId);
}

/// <summary>
/// Sayfalı sonuç için generic class
/// </summary>
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