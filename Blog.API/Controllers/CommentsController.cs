using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<CommentsController> _logger;

    public CommentsController(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<CommentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("posts/{postId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetPostComments(Guid postId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var post = await _unitOfWork.Posts.GetByIdAsync(postId);
            if (post == null)
                return NotFound("Post bulunamadı");

            var comments = await _unitOfWork.Comments.FindAsync(c => c.PostId == postId);
            var rootComments = comments.Where(c => c.ParentCommentId == null).ToList();
            var totalCount = rootComments.Count;
            var pagedComments = rootComments
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var commentResponses = new List<object>();

            foreach (var comment in pagedComments)
            {
                var author = await _unitOfWork.Users.GetByIdAsync(comment.AuthorId);
                var replies = comments.Where(c => c.ParentCommentId == comment.Id).ToList();
                var replyResponses = new List<object>();

                foreach (var reply in replies)
                {
                    var replyAuthor = await _unitOfWork.Users.GetByIdAsync(reply.AuthorId);
                    replyResponses.Add(new
                    {
                        reply.Id,
                        reply.Content,
                        reply.CreatedAt,
                        reply.LikeCount,
                        Author = new
                        {
                            replyAuthor?.Id,
                            replyAuthor?.UserName,
                            replyAuthor?.FirstName,
                            replyAuthor?.LastName,
                            replyAuthor?.ProfileImageUrl
                        }
                    });
                }

                commentResponses.Add(new
                {
                    comment.Id,
                    comment.Content,
                    comment.CreatedAt,
                    comment.LikeCount,
                    Author = new
                    {
                        author?.Id,
                        author?.UserName,
                        author?.FirstName,
                        author?.LastName,
                        author?.ProfileImageUrl
                    },
                    Replies = replyResponses,
                    ReplyCount = replies.Count
                });
            }

            return Ok(new
            {
                Comments = commentResponses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments for post {PostId}", postId);
            return StatusCode(500, new { Error = "Yorumlar getirilirken hata oluştu" });
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _unitOfWork.Posts.GetByIdAsync(request.PostId);
            if (post == null)
                return NotFound("Post bulunamadı");

            // Parent comment kontrolü
            if (request.ParentCommentId.HasValue)
            {
                var parentComment = await _unitOfWork.Comments.GetByIdAsync(request.ParentCommentId.Value);
                if (parentComment == null)
                    return NotFound("Üst yorum bulunamadı");
            }

            var comment = new Comment
            {
                Content = request.Content,
                PostId = request.PostId,
                AuthorId = userId,
                ParentCommentId = request.ParentCommentId
            };

            await _unitOfWork.Comments.AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.Posts.UpdateCommentCountAsync(request.PostId);
            await _unitOfWork.SaveChangesAsync();

            if (post.AuthorId != userId)
            {
                await _notificationService.SendNewCommentNotificationAsync(
                    request.PostId, comment.Id, userId, post.AuthorId);
            }

            var author = await _unitOfWork.Users.GetByIdAsync(userId);

            _logger.LogInformation("Comment created: {CommentId} on post {PostId} by {UserId}", 
                comment.Id, request.PostId, userId);

            return CreatedAtAction(nameof(GetCommentById), new { id = comment.Id }, new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.LikeCount,
                Author = new
                {
                    author?.Id,
                    author?.UserName,
                    author?.FirstName,
                    author?.LastName,
                    author?.ProfileImageUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating comment");
            return StatusCode(500, new { Error = "Yorum oluşturulurken hata oluştu" });
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> UpdateComment(Guid id, [FromBody] UpdateCommentRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound("Yorum bulunamadı");

            if (comment.AuthorId != userId)
                return Forbid("Sadece yorum sahibi bu işlemi yapabilir");

            comment.Content = request.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Comments.Update(comment);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comment updated: {CommentId} by {UserId}", id, userId);

            return Ok(new { Message = "Yorum güncellendi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating comment {CommentId}", id);
            return StatusCode(500, new { Error = "Yorum güncellenirken hata oluştu" });
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound("Yorum bulunamadı");

            if (comment.AuthorId != userId)
                return Forbid("Sadece yorum sahibi bu işlemi yapabilir");

            _unitOfWork.Comments.Remove(comment);
            await _unitOfWork.SaveChangesAsync();

            await _unitOfWork.Posts.UpdateCommentCountAsync(comment.PostId);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Comment deleted: {CommentId} by {UserId}", id, userId);

            return Ok(new { Message = "Yorum silindi" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", id);
            return StatusCode(500, new { Error = "Yorum silinirken hata oluştu" });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCommentById(Guid id)
    {
        try
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null)
                return NotFound("Yorum bulunamadı");

            var author = await _unitOfWork.Users.GetByIdAsync(comment.AuthorId);

            return Ok(new
            {
                comment.Id,
                comment.Content,
                comment.CreatedAt,
                comment.UpdatedAt,
                comment.LikeCount,
                comment.PostId,
                comment.ParentCommentId,
                Author = new
                {
                    author?.Id,
                    author?.UserName,
                    author?.FirstName,
                    author?.LastName,
                    author?.ProfileImageUrl
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment {CommentId}", id);
            return StatusCode(500, new { Error = "Yorum getirilirken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
}

public class CreateCommentRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid? ParentCommentId { get; set; }
}

public class UpdateCommentRequest
{
    public string Content { get; set; } = string.Empty;
} 