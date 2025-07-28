using Blog.Application.Common.Interfaces;
using Blog.Application.Features.Posts.DTOs;
using Blog.Application.Features.Posts.Interfaces;
using Blog.Application.Features.Authentication.DTOs;
using Blog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Blog.Application.Features.Posts.Services;

/// <summary>
/// Post işlemleri business logic implementation
/// </summary>
public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PostResponse> CreatePostAsync(CreatePostRequest request, string authorId)
    {
        // Slug oluştur
        var slug = GenerateSlug(request.Title);
        
        // Slug unique mi kontrol et
        var existingPost = await _unitOfWork.Posts.GetBySlugAsync(slug);
        if (existingPost != null)
        {
            slug = $"{slug}-{DateTime.UtcNow.Ticks}";
        }

        // Reading time hesapla (ortalama 200 kelime/dakika)
        var readTimeMinutes = CalculateReadTime(request.Content);

        var post = new Post
        {
            Title = request.Title,
            Slug = slug,
            Content = request.Content,
            Excerpt = request.Excerpt ?? GenerateExcerpt(request.Content),
            FeaturedImageUrl = request.FeaturedImageUrl,
            Status = request.Status,
            AuthorId = authorId,
            CategoryId = request.CategoryId,
            ReadTimeMinutes = readTimeMinutes,
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            PublishedAt = request.Status == PostStatus.Published ? DateTime.UtcNow : null
        };

        await _unitOfWork.Posts.AddAsync(post);

        // Tag'leri işle
        if (request.Tags?.Any() == true)
        {
            await ProcessPostTagsAsync(post.Id, request.Tags);
        }

        await _unitOfWork.SaveChangesAsync();

        return await MapToPostResponseAsync(post);
    }

    public async Task<PostResponse> UpdatePostAsync(Guid postId, UpdatePostRequest request)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post bulunamadı");

        // Slug güncelle (title değiştiyse)
        if (post.Title != request.Title)
        {
            var newSlug = GenerateSlug(request.Title);
            var existingPost = await _unitOfWork.Posts.GetBySlugAsync(newSlug);
            if (existingPost != null && existingPost.Id != postId)
            {
                newSlug = $"{newSlug}-{DateTime.UtcNow.Ticks}";
            }
            post.Slug = newSlug;
        }

        post.Title = request.Title;
        post.Content = request.Content;
        post.Excerpt = request.Excerpt ?? GenerateExcerpt(request.Content);
        post.FeaturedImageUrl = request.FeaturedImageUrl;
        post.CategoryId = request.CategoryId;
        post.ReadTimeMinutes = CalculateReadTime(request.Content);
        post.MetaDescription = request.MetaDescription;
        post.MetaKeywords = request.MetaKeywords;

        // Status değişikliği
        var oldStatus = post.Status;
        post.Status = request.Status;
        
        if (oldStatus != PostStatus.Published && request.Status == PostStatus.Published)
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        _unitOfWork.Posts.Update(post);

        // Tag'leri güncelle
        await ProcessPostTagsAsync(post.Id, request.Tags);

        await _unitOfWork.SaveChangesAsync();

        return await MapToPostResponseAsync(post);
    }

    public async Task DeletePostAsync(Guid postId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post bulunamadı");

        _unitOfWork.Posts.Remove(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PostResponse?> GetPostByIdAsync(Guid postId)
    {
        var posts = await _unitOfWork.Posts.GetWithIncludeAsync(
            p => p.Id == postId,
            p => p.Author,
            p => p.Category,
            p => p.PostTags!
        );
        
        var post = posts.FirstOrDefault();
        return post != null ? await MapToPostResponseAsync(post) : null;
    }

    public async Task<PostResponse?> GetPostBySlugAsync(string slug)
    {
        var post = await _unitOfWork.Posts.GetBySlugAsync(slug);
        if (post != null)
        {
            // View count artır
            await _unitOfWork.Posts.UpdateViewCountAsync(post.Id);
        }

        return post != null ? await MapToPostResponseAsync(post) : null;
    }

    public async Task<PagedResult<PostResponse>> GetPublishedPostsAsync(int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetPublishedPostsAsync(page, pageSize);
        
        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PostResponse>> GetUserPostsAsync(string userId, int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetUserPostsAsync(userId, page, pageSize);

        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PostResponse>> GetPostsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetPostsByCategoryPagedAsync(categoryId, page, pageSize);

        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PostResponse>> GetPostsByCategorySlugAsync(string categorySlug, int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetPostsByCategorySlugAsync(categorySlug, page, pageSize);

        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PostResponse>> GetPostsByTagAsync(string tagSlug, int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.GetPostsByTagSlugAsync(tagSlug, page, pageSize);

        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<PostResponse>> SearchPostsAsync(string query, int page = 1, int pageSize = 10)
    {
        var (posts, totalCount) = await _unitOfWork.Posts.SearchPostsAsync(query, page, pageSize);
        
        var postResponses = new List<PostResponse>();
        foreach (var post in posts)
        {
            postResponses.Add(await MapToPostResponseAsync(post));
        }

        return new PagedResult<PostResponse>
        {
            Items = postResponses,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PostResponse> PublishPostAsync(Guid postId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post bulunamadı");

        post.Status = PostStatus.Published;
        post.PublishedAt = DateTime.UtcNow;

        _unitOfWork.Posts.Update(post);
        await _unitOfWork.SaveChangesAsync();

        return await MapToPostResponseAsync(post);
    }

    public async Task<PostResponse> UnpublishPostAsync(Guid postId)
    {
        var post = await _unitOfWork.Posts.GetByIdAsync(postId);
        if (post == null)
            throw new KeyNotFoundException("Post bulunamadı");

        post.Status = PostStatus.Draft;
        post.PublishedAt = null;

        _unitOfWork.Posts.Update(post);
        await _unitOfWork.SaveChangesAsync();

        return await MapToPostResponseAsync(post);
    }

    #region Helper Methods

    private static string GenerateSlug(string title)
    {
        // Türkçe karakterleri dönüştür
        string slug = title.ToLowerInvariant();
        slug = slug.Replace("ş", "s").Replace("ğ", "g").Replace("ü", "u")
                  .Replace("ı", "i").Replace("ö", "o").Replace("ç", "c");
        
        // Özel karakterleri kaldır ve boşlukları tire ile değiştir
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');
        
        return slug;
    }

    private static string GenerateExcerpt(string content, int maxLength = 300)
    {
        if (string.IsNullOrWhiteSpace(content))
            return string.Empty;

        // HTML tag'lerini temizle (basit)
        string plainText = Regex.Replace(content, "<.*?>", "");
        
        if (plainText.Length <= maxLength)
            return plainText;

        return plainText.Substring(0, maxLength) + "...";
    }

    private static int CalculateReadTime(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            return 1;

        // HTML tag'lerini temizle
        string plainText = Regex.Replace(content, "<.*?>", "");
        
        // Kelime sayısını hesapla
        int wordCount = plainText.Split(new[] { ' ', '\t', '\n', '\r' }, 
            StringSplitOptions.RemoveEmptyEntries).Length;
        
        // Ortalama 200 kelime/dakika
        int readTime = (int)Math.Ceiling(wordCount / 200.0);
        
        return Math.Max(readTime, 1); // En az 1 dakika
    }

    private async Task ProcessPostTagsAsync(Guid postId, List<string> tagNames)
    {
        // Mevcut tag'leri sil
        var existingPostTags = await _unitOfWork.PostTags.FindAsync(pt => pt.PostId == postId);
        foreach (var postTag in existingPostTags)
        {
            _unitOfWork.PostTags.Remove(postTag);
        }

        // Yeni tag'leri ekle
        foreach (var tagName in tagNames.Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            var slug = GenerateSlug(tagName);
            
            // Tag var mı kontrol et
            var tag = await _unitOfWork.Tags.FindAsync(t => t.Slug == slug);
            var existingTag = tag.FirstOrDefault();
            
            if (existingTag == null)
            {
                // Yeni tag oluştur
                existingTag = new Tag
                {
                    Name = tagName.Trim(),
                    Slug = slug,
                    PostCount = 1
                };
                await _unitOfWork.Tags.AddAsync(existingTag);
            }
            else
            {
                // Var olan tag'in post count'unu artır
                existingTag.PostCount++;
                _unitOfWork.Tags.Update(existingTag);
            }

            // PostTag ilişkisi oluştur
            var postTag = new PostTag
            {
                PostId = postId,
                TagId = existingTag.Id
            };
            await _unitOfWork.PostTags.AddAsync(postTag);
        }
    }

    private async Task<PostResponse> MapToPostResponseAsync(Post post)
    {
        // Author bilgisini al
        var author = await _unitOfWork.Users.GetByIdAsync(post.AuthorId);
        
        // Category bilgisini al
        Category? category = null;
        if (post.CategoryId.HasValue)
        {
            category = await _unitOfWork.Categories.GetByIdAsync(post.CategoryId.Value);
        }

        // Tag'leri al
        var postTags = await _unitOfWork.PostTags.FindAsync(pt => pt.PostId == post.Id);
        var tags = new List<TagResponse>();
        
        foreach (var postTag in postTags)
        {
            var tag = await _unitOfWork.Tags.GetByIdAsync(postTag.TagId);
            if (tag != null)
            {
                tags.Add(new TagResponse
                {
                    Id = tag.Id,
                    Name = tag.Name,
                    Slug = tag.Slug,
                    Color = tag.Color
                });
            }
        }

        return new PostResponse
        {
            Id = post.Id,
            Title = post.Title,
            Slug = post.Slug,
            Content = post.Content,
            Excerpt = post.Excerpt,
            FeaturedImageUrl = post.FeaturedImageUrl,
            Status = post.Status,
            PublishedAt = post.PublishedAt,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            ViewCount = post.ViewCount,
            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            ReadTimeMinutes = post.ReadTimeMinutes,
            MetaDescription = post.MetaDescription,
            MetaKeywords = post.MetaKeywords,
            Author = new UserDto
            {
                Id = author?.Id ?? string.Empty,
                UserName = author?.UserName ?? string.Empty,
                Email = author?.Email ?? string.Empty,
                FirstName = author?.FirstName,
                LastName = author?.LastName,
                Bio = author?.Bio,
                ProfileImageUrl = author?.ProfileImageUrl
            },
            Category = category != null ? new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Slug = category.Slug,
                Color = category.Color
            } : null,
            Tags = tags
        };
    }

    #endregion
} 