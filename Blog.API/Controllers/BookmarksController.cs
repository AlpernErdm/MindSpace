using Blog.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class BookmarksController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BookmarksController> _logger;

    public BookmarksController(
        IUnitOfWork unitOfWork,
        ILogger<BookmarksController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetBookmarks(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            // var bookmarks = await _unitOfWork.Bookmarks.FindAsync(b => b.UserId == userId);
            var bookmarks = (await _unitOfWork.Bookmarks.GetAllAsync())
                .Where(b => b.UserId == userId)
                .ToList();
            var postIds = bookmarks.Select(b => b.PostId).ToList();
            
            // var posts = await _unitOfWork.Posts.FindAsync(p => postIds.Contains(p.Id));
            var posts = (await _unitOfWork.Posts.GetAllAsync())
                .Where(p => postIds.Contains(p.Id))
                .ToList();
            var pagedPosts = posts
                .OrderByDescending(p => p.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var totalCount = posts.Count;

            var response = new
            {
                Bookmarks = pagedPosts.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.Excerpt,
                    p.Content,
                    p.FeaturedImageUrl,
                    p.ViewCount,
                    p.LikeCount,
                    p.CommentCount,
                    p.ReadTimeMinutes,
                    p.PublishedAt,
                    p.CreatedAt,
                    p.UpdatedAt,
                    Category = p.Category != null ? new
                    {
                        p.Category.Id,
                        p.Category.Name,
                        p.Category.Slug
                    } : null,
                    Author = p.Author != null ? new
                    {
                        p.Author.Id,
                        p.Author.UserName,
                        p.Author.FirstName,
                        p.Author.LastName,
                        p.Author.ProfileImageUrl,
                        p.Author.Bio,
                        p.Author.FollowerCount,
                        p.Author.FollowingCount
                    } : null,
                    Tags = new List<object>()
                }),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookmarks for user");
            return StatusCode(500, new { Message = "Kaydedilen yazılar alınırken hata oluştu" });
        }
    }

    [HttpPost("{postId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleBookmark(Guid postId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _unitOfWork.Posts.GetByIdAsync(postId);
            if (post == null)
                return NotFound("Post bulunamadı");

            var existingBookmark = await _unitOfWork.Bookmarks
                .FindAsync(b => b.UserId == userId && b.PostId == postId);

            if (existingBookmark.Any())
            {
                // Bookmark'ı kaldır
                var bookmark = existingBookmark.First();
                _unitOfWork.Bookmarks.Remove(bookmark);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Bookmark removed for user {UserId} and post {PostId}", userId, postId);

                return Ok(new { IsBookmarked = false, Message = "Yazı kaydedilenlerden çıkarıldı" });
            }
            else
            {
                // Bookmark ekle
                var bookmark = new Domain.Entities.Bookmark
                {
                    UserId = userId,
                    PostId = postId
                };

                await _unitOfWork.Bookmarks.AddAsync(bookmark);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Bookmark added for user {UserId} and post {PostId}", userId, postId);

                return Ok(new { IsBookmarked = true, Message = "Yazı kaydedildi" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling bookmark for post {PostId}", postId);
            return StatusCode(500, new { Message = "Bookmark işlemi sırasında hata oluştu" });
        }
    }

    [HttpGet("{postId}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetBookmarkStatus(Guid postId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var bookmark = await _unitOfWork.Bookmarks
                .FindAsync(b => b.UserId == userId && b.PostId == postId);

            var isBookmarked = bookmark.Any();

            return Ok(new { IsBookmarked = isBookmarked });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookmark status for post {PostId}", postId);
            return StatusCode(500, new { Message = "Bookmark durumu alınırken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 