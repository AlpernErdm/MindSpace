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
public class UsersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUnitOfWork unitOfWork,
        INotificationService notificationService,
        ILogger<UsersController> logger)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpGet("{userName}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserProfile(string userName)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            var recentPosts = await _unitOfWork.Posts.FindAsync(p => 
                p.AuthorId == user.Id && p.Status == PostStatus.Published);
            var recentPostsList = recentPosts
                .OrderByDescending(p => p.PublishedAt)
                .Take(5)
                .ToList();

            var currentUserId = GetCurrentUserId();
            var isFollowing = false;

            if (!string.IsNullOrEmpty(currentUserId) && currentUserId != user.Id)
            {
                isFollowing = await _unitOfWork.Users.IsFollowingAsync(currentUserId, user.Id);
            }

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                user.Bio,
                user.ProfileImageUrl,
                user.Website,
                user.TwitterHandle,
                user.LinkedInUrl,
                user.JoinDate,
                user.FollowerCount,
                user.FollowingCount,
                user.IsVerified,
                IsFollowing = isFollowing,
                RecentPosts = recentPostsList.Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Slug,
                    p.Excerpt,
                    p.PublishedAt,
                    p.ViewCount,
                    p.LikeCount,
                    p.CommentCount
                })
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile for {UserName}", userName);
            return StatusCode(500, new { Error = "Kullanıcı profili getirilirken hata oluştu" });
        }
    }

    [HttpPost("{userName}/follow")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ToggleFollow(string userName)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            if (string.IsNullOrEmpty(currentUserId))
                return Unauthorized();

            var userToFollow = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (userToFollow == null)
                return NotFound("Kullanıcı bulunamadı");

            // Kendini takip etmeye çalışıyorsa hata ver
            if (currentUserId == userToFollow.Id)
                return BadRequest("Kendinizi takip edemezsiniz");

            var isFollowing = await _unitOfWork.Users.IsFollowingAsync(currentUserId, userToFollow.Id);

            if (isFollowing)
            {
                // Unfollow
                await _unitOfWork.Users.UnfollowUserAsync(currentUserId, userToFollow.Id);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("User unfollowed: {FollowerId} unfollowed {FollowingId}", 
                    currentUserId, userToFollow.Id);

                return Ok(new { 
                    Message = "Takip bırakıldı",
                    IsFollowing = false
                });
            }
            else
            {
                // Follow
                await _unitOfWork.Users.FollowUserAsync(currentUserId, userToFollow.Id);
                await _unitOfWork.SaveChangesAsync();

                // Send notification
                await _notificationService.SendNewFollowerNotificationAsync(currentUserId, userToFollow.Id);

                _logger.LogInformation("User followed: {FollowerId} followed {FollowingId}", 
                    currentUserId, userToFollow.Id);

                return Ok(new { 
                    Message = "Kullanıcı takip edildi",
                    IsFollowing = true
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling follow for user {UserName}", userName);
            return StatusCode(500, new { Error = "Takip işlemi sırasında hata oluştu" });
        }
    }
    [HttpGet("{userName}/followers")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFollowers(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            var followers = await _unitOfWork.Users.GetFollowersAsync(user.Id);
            var totalCount = followers.Count();
            var pagedFollowers = followers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var followerResponses = pagedFollowers.Select(f => new
            {
                f.Id,
                f.UserName,
                f.FirstName,
                f.LastName,
                f.Bio,
                f.ProfileImageUrl,
                f.FollowerCount,
                f.FollowingCount,
                f.IsVerified
            });

            return Ok(new
            {
                Followers = followerResponses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting followers for user {UserName}", userName);
            return StatusCode(500, new { Error = "Takipçiler getirilirken hata oluştu" });
        }
    }

    [HttpGet("{userName}/following")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetFollowing(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            var following = await _unitOfWork.Users.GetFollowingAsync(user.Id);
            var totalCount = following.Count();
            var pagedFollowing = following
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var followingResponses = pagedFollowing.Select(f => new
            {
                f.Id,
                f.UserName,
                f.FirstName,
                f.LastName,
                f.Bio,
                f.ProfileImageUrl,
                f.FollowerCount,
                f.FollowingCount,
                f.IsVerified
            });

            return Ok(new
            {
                Following = followingResponses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting following for user {UserName}", userName);
            return StatusCode(500, new { Error = "Takip edilenler getirilirken hata oluştu" });
        }
    }
    [HttpGet("{userName}/posts")]
    [ProducesResponseType(200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUserPosts(string userName, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByUserNameAsync(userName);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı");

            var posts = await _unitOfWork.Posts.FindAsync(p => 
                p.AuthorId == user.Id && p.Status == PostStatus.Published);
            var totalCount = posts.Count();
            var pagedPosts = posts
                .OrderByDescending(p => p.PublishedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var postResponses = pagedPosts.Select(p => new
            {
                p.Id,
                p.Title,
                p.Slug,
                p.Excerpt,
                p.FeaturedImageUrl,
                p.PublishedAt,
                p.ViewCount,
                p.LikeCount,
                p.CommentCount,
                p.ReadTimeMinutes,
                Category = p.Category != null ? new
                {
                    p.Category.Id,
                    p.Category.Name,
                    p.Category.Slug
                } : null,
                Tags = p.PostTags?.Select(pt => new
                {
                    pt.Tag.Id,
                    pt.Tag.Name,
                    pt.Tag.Slug
                }).ToList()
            });

            return Ok(new
            {
                Posts = postResponses,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts for user {UserName}", userName);
            return StatusCode(500, new { Error = "Postlar getirilirken hata oluştu" });
        }
    }

    #region Helper Methods

    private string? GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    #endregion
} 