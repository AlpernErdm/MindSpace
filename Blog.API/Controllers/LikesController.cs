using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class LikesController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<LikesController> _logger;

    public LikesController(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<LikesController> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }
    [HttpPost("posts/{postId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> TogglePostLike(Guid postId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var post = await _unitOfWork.Posts.GetByIdAsync(postId);
            if (post == null)
                return NotFound("Post bulunamadı");

            var existingLike = await _unitOfWork.Likes.FindAsync(l => 
                l.UserId == userId && l.PostId == postId);

            if (existingLike.Any())
            {
                var like = existingLike.First();
                _unitOfWork.Likes.Remove(like);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.Posts.UpdateLikeCountAsync(postId);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Post unliked: {PostId} by {UserId}", postId, userId);
                
                return Ok(new { 
                    Message = "Beğeni kaldırıldı",
                    IsLiked = false,
                    LikeCount = post.LikeCount - 1
                });
            }
            else
            {
                var like = new Like
                {
                    UserId = userId,
                    PostId = postId,
                    Type = LikeType.Like
                };

                await _unitOfWork.Likes.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.Posts.UpdateLikeCountAsync(postId);
                await _unitOfWork.SaveChangesAsync();

                if (post.AuthorId != userId)
                {
                    await _notificationService.SendPostLikedNotificationAsync(postId, userId, post.AuthorId);
                }

                _logger.LogInformation("Post liked: {PostId} by {UserId}", postId, userId);
                
                return Ok(new { 
                    Message = "Post beğenildi",
                    IsLiked = true,
                    LikeCount = post.LikeCount + 1
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling post like for post {PostId}", postId);
            return StatusCode(500, new { Error = "Beğeni işlemi sırasında hata oluştu" });
        }
    }

    [HttpPost("comments/{commentId}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleCommentLike(Guid commentId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var comment = await _unitOfWork.Comments.GetByIdAsync(commentId);
            if (comment == null)
                return NotFound("Yorum bulunamadı");

            var existingLike = await _unitOfWork.Likes.FindAsync(l => 
                l.UserId == userId && l.CommentId == commentId);

            if (existingLike.Any())
            {
                var like = existingLike.First();
                _unitOfWork.Likes.Remove(like);
                await _unitOfWork.SaveChangesAsync();

                comment.LikeCount = Math.Max(0, comment.LikeCount - 1);
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Comment unliked: {CommentId} by {UserId}", commentId, userId);
                
                return Ok(new { 
                    Message = "Beğeni kaldırıldı",
                    IsLiked = false,
                    LikeCount = comment.LikeCount
                });
            }
            else
            {
                var like = new Like
                {
                    UserId = userId,
                    CommentId = commentId,
                    Type = LikeType.Like
                };

                await _unitOfWork.Likes.AddAsync(like);
                await _unitOfWork.SaveChangesAsync();

                comment.LikeCount++;
                _unitOfWork.Comments.Update(comment);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Comment liked: {CommentId} by {UserId}", commentId, userId);
                
                return Ok(new { 
                    Message = "Yorum beğenildi",
                    IsLiked = true,
                    LikeCount = comment.LikeCount
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling comment like for comment {CommentId}", commentId);
            return StatusCode(500, new { Error = "Beğeni işlemi sırasında hata oluştu" });
        }
    }

    [HttpGet("posts/{postId}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetPostLikeStatus(Guid postId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var existingLike = await _unitOfWork.Likes.FindAsync(l => 
                l.UserId == userId && l.PostId == postId);

            return Ok(new { IsLiked = existingLike.Any() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting post like status for post {PostId}", postId);
            return StatusCode(500, new { Error = "Beğeni durumu kontrol edilirken hata oluştu" });
        }
    }

    [HttpGet("comments/{commentId}/status")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> GetCommentLikeStatus(Guid commentId)
    {
        try
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var existingLike = await _unitOfWork.Likes.FindAsync(l => 
                l.UserId == userId && l.CommentId == commentId);

            return Ok(new { IsLiked = existingLike.Any() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comment like status for comment {CommentId}", commentId);
            return StatusCode(500, new { Error = "Beğeni durumu kontrol edilirken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 