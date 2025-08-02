using Blog.Application.Features.Posts.DTOs;
using Blog.Application.Features.Posts.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IPostService postService, ILogger<PostsController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Author,Admin")]
    [ProducesResponseType(typeof(PostResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("Model validation failed: {Errors}", string.Join(", ", errors));
                return BadRequest(new { Errors = errors });
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }

            var post = await _postService.CreatePostAsync(request, userId);
            
            _logger.LogInformation("📝 Post created: {PostId} by {UserId}", post.Id, userId);
            
            return CreatedAtAction(nameof(GetPostById), new { id = post.Id }, post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            return StatusCode(500, new { Error = "Post oluşturulurken hata oluştu" });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Author,Admin")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePost(Guid id, [FromBody] UpdatePostRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound("Post bulunamadı");
            }

            var userRoles = GetCurrentUserRoles();
            var isAdmin = userRoles.Contains("Admin");
            var isOwner = existingPost.Author.Id == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Sadece post sahibi veya admin bu işlemi yapabilir");
            }

            var updatedPost = await _postService.UpdatePostAsync(id, request);
            
            _logger.LogInformation("✏️ Post updated: {PostId} by {UserId}", id, userId);
            
            return Ok(updatedPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating post {PostId}", id);
            return StatusCode(500, new { Error = "Post güncellenirken hata oluştu" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Author,Admin")]
    [ProducesResponseType(204)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }
            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound("Post bulunamadı");
            }

            var userRoles = GetCurrentUserRoles();
            var isAdmin = userRoles.Contains("Admin");
            var isOwner = existingPost.Author.Id == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Sadece post sahibi veya admin bu işlemi yapabilir");
            }

            await _postService.DeletePostAsync(id);
            
            _logger.LogInformation("🗑️ Post deleted: {PostId} by {UserId}", id, userId);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting post {PostId}", id);
            return StatusCode(500, new { Error = "Post silinirken hata oluştu" });
        }
    }
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPostById(Guid id)
    {
        try
        {
            var post = await _postService.GetPostByIdAsync(id);
            
            if (post == null)
            {
                return NotFound("Post bulunamadı");
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post getirme hatası - PostId: {PostId}", id);
            return StatusCode(500, "Post getirilirken bir hata oluştu");
        }
    }

    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPostBySlug(string slug)
    {
        try
        {
            var post = await _postService.GetPostBySlugAsync(slug);
            
            if (post == null)
            {
                return NotFound("Post bulunamadı");
            }

            return Ok(post);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post getirme hatası - Slug: {Slug}", slug);
            return StatusCode(500, "Post getirilirken bir hata oluştu");
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    public async Task<IActionResult> GetPublishedPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.GetPublishedPostsAsync(page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post listesi getirme hatası");
            return StatusCode(500, "Post listesi getirilirken bir hata oluştu");
        }
    }

    [HttpGet("my-posts")]
    [Authorize]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetMyPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.GetUserPostsAsync(userId, page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kullanıcı postları getirme hatası");
            return StatusCode(500, "Postlar getirilirken bir hata oluştu");
        }
    }

    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    public async Task<IActionResult> GetPostsByCategory(Guid categoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.GetPostsByCategoryAsync(categoryId, page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori postları getirme hatası - CategoryId: {CategoryId}", categoryId);
            return StatusCode(500, "Kategori postları getirilirken bir hata oluştu");
        }
    }

    [HttpGet("category/slug/{categorySlug}")]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    public async Task<IActionResult> GetPostsByCategorySlug(string categorySlug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.GetPostsByCategorySlugAsync(categorySlug, page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Kategori postları getirme hatası - CategorySlug: {CategorySlug}", categorySlug);
            return StatusCode(500, "Kategori postları getirilirken bir hata oluştu");
        }
    }

    [HttpGet("tag/{tagSlug}")]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    public async Task<IActionResult> GetPostsByTag(string tagSlug, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.GetPostsByTagAsync(tagSlug, page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tag postları getirme hatası - TagSlug: {TagSlug}", tagSlug);
            return StatusCode(500, "Tag postları getirilirken bir hata oluştu");
        }
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedResult<PostResponse>), 200)]
    public async Task<IActionResult> SearchPosts([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Arama terimi boş olamaz");
            }

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _postService.SearchPostsAsync(query, page, pageSize);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post arama hatası - Query: {Query}", query);
            return StatusCode(500, "Arama yapılırken bir hata oluştu");
        }
    }

    [HttpPost("{id}/publish")]
    [Authorize(Roles = "Author,Admin")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> PublishPost(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }

            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound("Post bulunamadı");
            }

            var userRoles = GetCurrentUserRoles();
            var isAdmin = userRoles.Contains("Admin");
            var isOwner = existingPost.Author.Id == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Sadece post sahibi veya admin bu işlemi yapabilir");
            }

            var publishedPost = await _postService.PublishPostAsync(id);
            
            _logger.LogInformation("📢 Post published: {PostId} by {UserId}", id, userId);
            
            return Ok(publishedPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing post {PostId}", id);
            return StatusCode(500, new { Error = "Post yayınlanırken hata oluştu" });
        }
    }

    [HttpPost("{id}/unpublish")]
    [Authorize(Roles = "Author,Admin")]
    [ProducesResponseType(typeof(PostResponse), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UnpublishPost(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Kullanıcı kimliği bulunamadı");
            }

            var existingPost = await _postService.GetPostByIdAsync(id);
            if (existingPost == null)
            {
                return NotFound("Post bulunamadı");
            }

            var userRoles = GetCurrentUserRoles();
            var isAdmin = userRoles.Contains("Admin");
            var isOwner = existingPost.Author.Id == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid("Sadece post sahibi veya admin bu işlemi yapabilir");
            }

            var unpublishedPost = await _postService.UnpublishPostAsync(id);
            
            _logger.LogInformation("📝 Post unpublished: {PostId} by {UserId}", id, userId);
            
            return Ok(unpublishedPost);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unpublishing post {PostId}", id);
            return StatusCode(500, new { Error = "Post yayından kaldırılırken hata oluştu" });
        }
    }

    #region Helper Methods

    /// <summary>
    /// JWT token'dan user ID'yi al
    /// </summary>
    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Helper method: Current User Roles
    /// </summary>
    private List<string> GetCurrentUserRoles()
    {
        return User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }

    #endregion
} 