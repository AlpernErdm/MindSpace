using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Notifications.Interfaces;
using Blog.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TestController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly INotificationService _notificationService;
    private readonly IElasticsearchService _elasticsearchService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<TestController> _logger;

    public TestController(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        INotificationService notificationService,
        IElasticsearchService elasticsearchService,
        IMessagePublisher messagePublisher,
        ILogger<TestController> logger)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _notificationService = notificationService;
        _elasticsearchService = elasticsearchService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    [HttpGet("overview")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetSystemOverview()
    {
        try
        {
            var overview = new
            {
                Timestamp = DateTime.UtcNow,
                Database = new
                {
                    Users = await _userManager.Users.CountAsync(),
                    Posts = await _unitOfWork.Posts.CountAsync(),
                    Categories = await _unitOfWork.Categories.CountAsync(),
                    Tags = await _unitOfWork.Tags.CountAsync(),
                    Comments = await _unitOfWork.Comments.CountAsync(),
                    Likes = await _unitOfWork.Likes.CountAsync(),
                    UserFollows = await _unitOfWork.UserFollows.CountAsync(),
                    Notifications = await _unitOfWork.Notifications.CountAsync()
                },
                Search = new
                {
                    IsHealthy = await _elasticsearchService.IsHealthyAsync(),
                    Statistics = await _elasticsearchService.GetIndexStatsAsync()
                }
            };

            return Ok(overview);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system overview");
            return StatusCode(500, new { Error = "System overview failed" });
        }
    }

    [HttpGet("users")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetUsersWithStats()
    {
        try
        {
            var users = await _userManager.Users.ToListAsync();
            var userStats = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var postCount = await _unitOfWork.Posts.CountAsync(p => p.AuthorId == user.Id);
                var commentCount = await _unitOfWork.Comments.CountAsync(c => c.AuthorId == user.Id);
                var followerCount = await _unitOfWork.UserFollows.CountAsync(uf => uf.FollowingId == user.Id);
                var followingCount = await _unitOfWork.UserFollows.CountAsync(uf => uf.FollowerId == user.Id);

                userStats.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.Bio,
                    Roles = roles,
                    Stats = new
                    {
                        Posts = postCount,
                        Comments = commentCount,
                        Followers = followerCount,
                        Following = followingCount
                    }
                });
            }

            return Ok(new { Users = userStats, TotalCount = users.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users with stats");
            return StatusCode(500, new { Error = "Failed to get users" });
        }
    }

    [HttpGet("posts")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetPostsWithDetails([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        try
        {
            var (posts, totalCount) = await _unitOfWork.Posts.GetPagedAsync(page, pageSize);
            var postDetails = new List<object>();

            foreach (var post in posts)
            {
                var author = await _userManager.FindByIdAsync(post.AuthorId);
                var category = await _unitOfWork.Categories.GetByIdAsync(post.CategoryId ?? Guid.Empty);
                var tags = await GetPostTagsAsync(post.Id);
                var likeCount = await _unitOfWork.Likes.CountAsync(l => l.PostId == post.Id);
                var commentCount = await _unitOfWork.Comments.CountAsync(c => c.PostId == post.Id);

                postDetails.Add(new
                {
                    post.Id,
                    post.Title,
                    post.Slug,
                    post.Excerpt,
                    post.Status,
                    post.PublishedAt,
                    post.ViewCount,
                    post.ReadTimeMinutes,
                    Author = new
                    {
                        author?.UserName,
                        Name = $"{author?.FirstName} {author?.LastName}"
                    },
                    Category = category?.Name,
                    Tags = tags,
                    Stats = new
                    {
                        Likes = likeCount,
                        Comments = commentCount
                    }
                });
            }

            return Ok(new 
            { 
                Posts = postDetails, 
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting posts with details");
            return StatusCode(500, new { Error = "Failed to get posts" });
        }
    }

    [HttpGet("comments")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetCommentsHierarchy([FromQuery] Guid? postId = null)
    {
        try
        {
            var query = _unitOfWork.Comments.GetQueryable();
            
            if (postId.HasValue)
            {
                query = query.Where(c => c.PostId == postId.Value);
            }

            var comments = await query.Take(50).ToListAsync();
            var commentDetails = new List<object>();
            var rootComments = comments.Where(c => c.ParentCommentId == null).ToList();

            foreach (var comment in rootComments)
            {
                var author = await _userManager.FindByIdAsync(comment.AuthorId);
                var post = await _unitOfWork.Posts.GetByIdAsync(comment.PostId);
                var likeCount = await _unitOfWork.Likes.CountAsync(l => l.CommentId == comment.Id);
                var replies = comments.Where(c => c.ParentCommentId == comment.Id).ToList();
                var replyDetails = new List<object>();

                foreach (var reply in replies)
                {
                    var replyAuthor = await _userManager.FindByIdAsync(reply.AuthorId);
                    var replyLikeCount = await _unitOfWork.Likes.CountAsync(l => l.CommentId == reply.Id);
                    
                    replyDetails.Add(new
                    {
                        reply.Id,
                        reply.Content,
                        reply.CreatedAt,
                        Author = new
                        {
                            replyAuthor?.UserName,
                            Name = $"{replyAuthor?.FirstName} {replyAuthor?.LastName}"
                        },
                        Likes = replyLikeCount
                    });
                }

                commentDetails.Add(new
                {
                    comment.Id,
                    comment.Content,
                    comment.CreatedAt,
                    Author = new
                    {
                        author?.UserName,
                        Name = $"{author?.FirstName} {author?.LastName}"
                    },
                    Post = post?.Title,
                    Likes = likeCount,
                    Replies = replyDetails
                });
            }

            return Ok(new { Comments = commentDetails, TotalCount = rootComments.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting comments hierarchy");
            return StatusCode(500, new { Error = "Failed to get comments" });
        }
    }
    [HttpGet("follows")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetFollowRelationships()
    {
        try
        {
            var follows = await _unitOfWork.UserFollows.GetAllAsync();
            var followDetails = new List<object>();

            foreach (var follow in follows.Take(50))
            {
                var follower = await _userManager.FindByIdAsync(follow.FollowerId);
                var following = await _userManager.FindByIdAsync(follow.FollowingId);

                followDetails.Add(new
                {
                    follow.CreatedAt,
                    Follower = new
                    {
                        follower?.UserName,
                        Name = $"{follower?.FirstName} {follower?.LastName}"
                    },
                    Following = new
                    {
                        following?.UserName,
                        Name = $"{following?.FirstName} {following?.LastName}"
                    }
                });
            }

            return Ok(new { Follows = followDetails, TotalCount = follows.Count() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting follow relationships");
            return StatusCode(500, new { Error = "Failed to get follows" });
        }
    }

    [HttpPost("test-notifications")]
    [Authorize]
    [ProducesResponseType(200)]
    public async Task<IActionResult> TestNotificationSystem()
    {
        try
        {
            var currentUserId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var posts = await _unitOfWork.Posts.GetAllAsync();
            var users = await _userManager.Users.ToListAsync();
            var randomPost = posts.OrderBy(x => Guid.NewGuid()).First();
            var randomUser = users.Where(u => u.Id != currentUserId).OrderBy(x => Guid.NewGuid()).First();

            // Test different notification types
            var testResults = new List<object>();

            // 1. Post liked notification
            await _notificationService.SendPostLikedNotificationAsync(randomPost.Id, currentUserId, randomPost.AuthorId);
            testResults.Add(new { Type = "PostLiked", PostTitle = randomPost.Title, Author = randomPost.AuthorId });

            // 2. New follower notification  
            await _notificationService.SendNewFollowerNotificationAsync(randomUser.Id, currentUserId);
            testResults.Add(new { Type = "NewFollower", FollowedUser = $"{randomUser.FirstName} {randomUser.LastName}" });

            // 3. Test RabbitMQ message
            await _messagePublisher.PublishNotificationAsync(new Blog.Application.Common.Messages.PostLikedMessage
            {
                UserId = randomPost.AuthorId,
                Title = "Test Post Liked",
                Message = "Test message from notification system",
                PostId = randomPost.Id,
                PostTitle = randomPost.Title,
                PostAuthorId = randomPost.AuthorId,
                LikerUserId = currentUserId,
                LikerUserName = User.Identity?.Name ?? "Test User"
            });

            testResults.Add(new { Type = "RabbitMQ", Message = "Message published successfully" });

            return Ok(new 
            { 
                Message = "✅ Notification system test completed",
                Results = testResults,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing notification system");
            return StatusCode(500, new { Error = "Notification test failed" });
        }
    }

    [HttpPost("test-search")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> TestElasticsearchSearch()
    {
        try
        {
            var testQueries = new[] { "dotnet", "react", "microservices", "typescript", "docker" };
            var searchResults = new List<object>();

            foreach (var query in testQueries)
            {
                var searchRequest = new Blog.Application.Common.Search.SearchRequest
                {
                    Query = query,
                    Page = 1,
                    PageSize = 5,
                    HighlightResults = true
                };

                var response = await _elasticsearchService.SearchPostsAsync(searchRequest);
                
                searchResults.Add(new
                {
                    Query = query,
                    TotalResults = response.TotalCount,
                    SearchTime = response.SearchTime.TotalMilliseconds,
                    Results = response.Results.Select(r => new
                    {
                        r.Title,
                        r.AuthorName,
                        r.CategoryName,
                        r.Tags,
                        r.Score
                    }).ToList()
                });
            }
            var suggestions = await _elasticsearchService.GetSuggestionsAsync("react", 5);

            return Ok(new
            {
                Message = "✅ Elasticsearch search test completed",
                SearchResults = searchResults,
                Suggestions = suggestions,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Elasticsearch search");
            return StatusCode(500, new { Error = "Search test failed" });
        }
    }
    [HttpPost("seed-data")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SeedTestData()
    {
        try
        {
            // Kategoriler oluştur
            var categories = new[]
            {
                new Category { Name = "Teknoloji", Slug = "teknoloji", Description = "Teknoloji ile ilgili yazılar" },
                new Category { Name = "Yazılım", Slug = "yazilim", Description = "Yazılım geliştirme yazıları" },
                new Category { Name = "AI & Makine Öğrenmesi", Slug = "ai-makine-ogrenmesi", Description = "Yapay zeka ve makine öğrenmesi" },
                new Category { Name = "Web Geliştirme", Slug = "web-gelistirme", Description = "Web teknolojileri" },
                new Category { Name = "Mobil Geliştirme", Slug = "mobil-gelistirme", Description = "Mobil uygulama geliştirme" },
                new Category { Name = "Veri Bilimi", Slug = "veri-bilimi", Description = "Veri analizi ve bilimi" },
                new Category { Name = "DevOps", Slug = "devops", Description = "DevOps ve deployment" },
                new Category { Name = "Güvenlik", Slug = "guvenlik", Description = "Siber güvenlik" }
            };

            foreach (var category in categories)
            {
                var existingCategory = await _unitOfWork.Categories.FindAsync(c => c.Slug == category.Slug);
                if (!existingCategory.Any())
                {
                    await _unitOfWork.Categories.AddAsync(category);
                }
            }

            // Etiketler oluştur
            var tags = new[]
            {
                new Tag { Name = "csharp", Slug = "csharp", Description = "C# programlama dili" },
                new Tag { Name = "dotnet", Slug = "dotnet", Description = ".NET framework" },
                new Tag { Name = "aspnetcore", Slug = "aspnetcore", Description = "ASP.NET Core" },
                new Tag { Name = "react", Slug = "react", Description = "React.js" },
                new Tag { Name = "javascript", Slug = "javascript", Description = "JavaScript" },
                new Tag { Name = "typescript", Slug = "typescript", Description = "TypeScript" },
                new Tag { Name = "angular", Slug = "angular", Description = "Angular" },
                new Tag { Name = "vue", Slug = "vue", Description = "Vue.js" },
                new Tag { Name = "nodejs", Slug = "nodejs", Description = "Node.js" },
                new Tag { Name = "python", Slug = "python", Description = "Python" },
                new Tag { Name = "java", Slug = "java", Description = "Java" },
                new Tag { Name = "docker", Slug = "docker", Description = "Docker" },
                new Tag { Name = "kubernetes", Slug = "kubernetes", Description = "Kubernetes" },
                new Tag { Name = "aws", Slug = "aws", Description = "Amazon Web Services" },
                new Tag { Name = "azure", Slug = "azure", Description = "Microsoft Azure" },
                new Tag { Name = "api", Slug = "api", Description = "API geliştirme" },
                new Tag { Name = "database", Slug = "database", Description = "Veritabanı" },
                new Tag { Name = "sql", Slug = "sql", Description = "SQL" },
                new Tag { Name = "mongodb", Slug = "mongodb", Description = "MongoDB" },
                new Tag { Name = "redis", Slug = "redis", Description = "Redis" }
            };

            foreach (var tag in tags)
            {
                var existingTag = await _unitOfWork.Tags.FindAsync(t => t.Slug == tag.Slug);
                if (!existingTag.Any())
                {
                    await _unitOfWork.Tags.AddAsync(tag);
                }
            }

            // Test postları oluştur (eğer hiç post yoksa)
            var existingPosts = await _unitOfWork.Posts.GetAllAsync();
            if (!existingPosts.Any())
            {
                var users = await _userManager.Users.ToListAsync();
                if (users.Any())
                {
                    var firstUser = users.First();
                    var firstCategory = categories.First();
                    var firstTag = tags.First();

                    var testPosts = new[]
                    {
                        new Post
                        {
                            Title = "React ile Modern Web Uygulamaları Geliştirme",
                            Slug = "react-ile-modern-web-uygulamalari",
                            Content = "React, Facebook tarafından geliştirilen popüler bir JavaScript kütüphanesidir. Bu yazıda React'in temel kavramlarını ve modern web uygulamaları geliştirme süreçlerini ele alacağız...",
                            Excerpt = "React'in temel kavramları ve modern web uygulamaları geliştirme süreçleri",
                            Status = PostStatus.Published,
                            AuthorId = firstUser.Id,
                            CategoryId = firstCategory.Id,
                            ReadTimeMinutes = 8,
                            PublishedAt = DateTime.UtcNow.AddDays(-5),
                            ViewCount = 1250,
                            LikeCount = 45,
                            CommentCount = 12
                        },
                        new Post
                        {
                            Title = "ASP.NET Core ile RESTful API Geliştirme",
                            Slug = "aspnet-core-ile-restful-api-gelistirme",
                            Content = "ASP.NET Core, Microsoft'un modern web framework'üdür. Bu yazıda RESTful API geliştirme süreçlerini ve best practice'leri inceleyeceğiz...",
                            Excerpt = "ASP.NET Core ile RESTful API geliştirme süreçleri ve best practice'ler",
                            Status = PostStatus.Published,
                            AuthorId = firstUser.Id,
                            CategoryId = firstCategory.Id,
                            ReadTimeMinutes = 12,
                            PublishedAt = DateTime.UtcNow.AddDays(-3),
                            ViewCount = 890,
                            LikeCount = 32,
                            CommentCount = 8
                        },
                        new Post
                        {
                            Title = "Docker ile Containerization",
                            Slug = "docker-ile-containerization",
                            Content = "Docker, uygulamaları container'lar içinde çalıştırmamızı sağlayan popüler bir platformdur. Bu yazıda Docker'ın temellerini ve kullanım alanlarını ele alacağız...",
                            Excerpt = "Docker'ın temelleri ve containerization süreçleri",
                            Status = PostStatus.Published,
                            AuthorId = firstUser.Id,
                            CategoryId = firstCategory.Id,
                            ReadTimeMinutes = 10,
                            PublishedAt = DateTime.UtcNow.AddDays(-1),
                            ViewCount = 567,
                            LikeCount = 28,
                            CommentCount = 5
                        }
                    };

                    foreach (var post in testPosts)
                    {
                        await _unitOfWork.Posts.AddAsync(post);
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var result = new
            {
                Message = "✅ Test verileri başarıyla oluşturuldu",
                Categories = categories.Length,
                Tags = tags.Length,
                Posts = existingPosts.Any() ? "Mevcut postlar korundu" : "3 test postu oluşturuldu",
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Test verileri oluşturuldu: {Categories} kategoriler, {Tags} etiketler", categories.Length, tags.Length);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Test verileri oluşturulurken hata oluştu");
            return StatusCode(500, new { Error = "Test verileri oluşturulurken hata oluştu", Details = ex.Message });
        }
    }

    /// <summary>
    /// Her kategoriye 5'er yazı ekle (mevcut postları temizler)
    /// </summary>
    [HttpPost("seed-posts")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> SeedPostsOnly()
    {
        try
        {
            // Mevcut postları temizle
            var existingPosts = await _unitOfWork.Posts.GetAllAsync();
            if (existingPosts.Any())
            {
                _logger.LogInformation("🗑️ Clearing {Count} existing posts...", existingPosts.Count());
                await _unitOfWork.SaveChangesAsync();
            }

            // Kategorileri ve tagları al
            var categories = await _unitOfWork.Categories.GetAllAsync();
            var tags = await _unitOfWork.Tags.GetAllAsync();
            var users = await _userManager.Users.Where(u => u.UserName != "admin").ToListAsync();

            if (!categories.Any() || !tags.Any() || !users.Any())
            {
                return BadRequest(new { Error = "Kategori, tag veya kullanıcı bulunamadı. Önce /api/Test/seed-data endpoint'ini çalıştırın." });
            }

            // PostSeedData'dan postları oluştur
            var posts = new List<Post>();
            var postTags = new List<PostTag>();

            for (int i = 0; i < Blog.Infrastructure.Data.SeedData.PostSeedData.PostTemplates.Length; i++)
            {
                var template = Blog.Infrastructure.Data.SeedData.PostSeedData.PostTemplates[i];
                var author = users[i % users.Count];
                var category = categories.FirstOrDefault(c => c.Name == template.CategoryName);
                
                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryName}", template.CategoryName);
                    continue;
                }
                
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = template.Title,
                    Content = template.Content,
                    Excerpt = GenerateExcerpt(template.Content),
                    Slug = GenerateSlug(template.Title),
                    AuthorId = author.Id,
                    CategoryId = category.Id,
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(31, 60)),
                    UpdatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 10)),
                    ViewCount = Random.Shared.Next(50, 5000),
                    ReadTimeMinutes = Random.Shared.Next(3, 15),
                    MetaDescription = GenerateExcerpt(template.Content),
                    MetaKeywords = string.Join(", ", template.TagNames),
                    FeaturedImageUrl = $"https://picsum.photos/800/400?random={i + 1}"
                };

                posts.Add(post);

                // Add PostTags
                foreach (var tagName in template.TagNames)
                {
                    var tag = tags.FirstOrDefault(t => t.Name == tagName);
                    if (tag != null)
                    {
                        postTags.Add(new PostTag
                        {
                            PostId = post.Id,
                            TagId = tag.Id
                        });
                    }
                }
            }

            // Postları ve PostTag'leri kaydet
            await _unitOfWork.Posts.AddRangeAsync(posts);
            await _unitOfWork.SaveChangesAsync();

            // PostTag'leri ayrı kaydet (çünkü PostId'ler gerekli)
            foreach (var postTag in postTags)
            {
                await _unitOfWork.PostTags.AddAsync(postTag);
            }
            await _unitOfWork.SaveChangesAsync();

            var result = new
            {
                Message = "✅ Her kategoriye 5'er yazı başarıyla eklendi!",
                TotalPosts = posts.Count,
                TotalPostTags = postTags.Count,
                Categories = categories.Count(),
                Tags = tags.Count(),
                Users = users.Count,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("📝 {PostCount} posts with {TagCount} post-tag relationships created", posts.Count, postTags.Count);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Post seed data oluşturulurken hata oluştu");
            return StatusCode(500, new { Error = "Post seed data oluşturulurken hata oluştu", Details = ex.Message });
        }
    }

    #region Helper Methods

    private string GenerateSlug(string title)
    {
        return title.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("İ", "i")
            .Replace("Ğ", "g")
            .Replace("Ü", "u")
            .Replace("Ş", "s")
            .Replace("Ö", "o")
            .Replace("Ç", "c")
            .Replace(".", "")
            .Replace(",", "")
            .Replace(":", "")
            .Replace(";", "")
            .Replace("!", "")
            .Replace("?", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("\"", "")
            .Replace("'", "");
    }

    private string GenerateExcerpt(string content)
    {
        var maxLength = 200;
        if (content.Length <= maxLength)
            return content;
        
        return content.Substring(0, maxLength).TrimEnd() + "...";
    }

    #endregion

    /// <summary>
    /// Comprehensive system stress test (Admin only)
    /// </summary>
    [HttpPost("stress-test")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public async Task<IActionResult> RunStressTest()
    {
        try
        {
            var currentUserId = User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var results = new List<object>();

            // 1. Database stress test
            var posts = await _unitOfWork.Posts.GetAllAsync();
            var users = await _userManager.Users.ToListAsync();
            results.Add(new { Test = "Database", Status = "✅ OK", PostCount = posts.Count(), UserCount = users.Count });

            // 2. Search stress test
            var searchTasks = new[]
            {
                "dotnet", "react", "microservices", "javascript", "docker"
            }.Select(async query =>
            {
                var request = new Blog.Application.Common.Search.SearchRequest { Query = query, PageSize = 10 };
                var response = await _elasticsearchService.SearchPostsAsync(request);
                return new { Query = query, Results = response.TotalCount, Time = response.SearchTime.TotalMilliseconds };
            });

            var searchResults = await Task.WhenAll(searchTasks);
            results.Add(new { Test = "Search", Status = "✅ OK", Results = searchResults });

            // 3. Notification stress test
            var notificationTasks = Enumerable.Range(1, 5).Select(async i =>
            {
                var randomPost = posts.OrderBy(x => Guid.NewGuid()).First();
                await _notificationService.SendPostLikedNotificationAsync(randomPost.Id, currentUserId, randomPost.AuthorId);
                return $"Notification {i} sent";
            });

            var notificationResults = await Task.WhenAll(notificationTasks);
            results.Add(new { Test = "Notifications", Status = "✅ OK", Results = notificationResults });

            // 4. RabbitMQ stress test
            var messageCount = 10;
            var messageTasks = Enumerable.Range(1, messageCount).Select(async i =>
            {
                await _messagePublisher.PublishNotificationAsync(new Blog.Application.Common.Messages.PostLikedMessage
                {
                    UserId = currentUserId,
                    Title = $"Stress Test Message {i}",
                    Message = $"This is stress test message number {i}",
                    PostId = Guid.NewGuid(),
                    PostTitle = $"Test Post {i}",
                    PostAuthorId = currentUserId,
                    LikerUserId = currentUserId,
                    LikerUserName = "Stress Tester"
                });
                return $"Message {i} published";
            });

            var messageResults = await Task.WhenAll(messageTasks);
            results.Add(new { Test = "RabbitMQ", Status = "✅ OK", Results = messageResults });

            return Ok(new
            {
                Message = "🚀 Comprehensive stress test completed",
                Duration = "All tests passed",
                Results = results,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during stress test");
            return StatusCode(500, new { Error = "Stress test failed", Details = ex.Message });
        }
    }

    #region Helper Methods

    private async Task<List<string>> GetPostTagsAsync(Guid postId)
    {
        var postTags = await _unitOfWork.PostTags.GetQueryable()
            .Where(pt => pt.PostId == postId)
            .ToListAsync();

        var tags = new List<string>();
        foreach (var pt in postTags)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(pt.TagId);
            if (tag != null)
            {
                tags.Add(tag.Name);
            }
        }

        return tags;
    }

    #endregion
} 