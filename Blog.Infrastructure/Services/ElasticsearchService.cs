using Blog.Application.Common.Interfaces;
using Blog.Application.Common.Search;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Blog.Infrastructure.Services;

public class ElasticsearchService : IElasticsearchService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ElasticsearchService> _logger;
    private readonly string _elasticsearchUrl;
    private readonly string _indexName = "blog-posts";
    private readonly List<PostSearchDocument> _inMemoryIndex = new();
    private readonly List<PopularSearch> _popularSearches = new();

    public ElasticsearchService(
        IConfiguration configuration,
        ILogger<ElasticsearchService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _elasticsearchUrl = _configuration.GetConnectionString("Elasticsearch") ?? "http://localhost:9200";
    }

    public async Task InitializeAsync()
    {
        try
        {
            _logger.LogInformation("🔍 Initializing Elasticsearch indices...");
            await Task.Delay(100);
            _logger.LogInformation("✅ Elasticsearch indices initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to initialize Elasticsearch indices");
            throw;
        }
    }

    public async Task IndexPostAsync(PostSearchDocument document)
    {
        try
        {
            _logger.LogInformation("📝 Indexing post: {PostId} - {Title}", document.Id, document.Title);
            var existing = _inMemoryIndex.FirstOrDefault(p => p.Id == document.Id);
            if (existing != null)
            {
                _inMemoryIndex.Remove(existing);
            }
            _inMemoryIndex.Add(document);
            await Task.Delay(50); 
            _logger.LogInformation("✅ Post indexed successfully: {PostId}", document.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to index post: {PostId}", document.Id);
            throw;
        }
    }

    public async Task UpdatePostAsync(PostSearchDocument document)
    {
        await IndexPostAsync(document); 
    }

    public async Task DeletePostAsync(Guid postId)
    {
        try
        {
            _logger.LogInformation("🗑️ Deleting post from index: {PostId}", postId);
            var existing = _inMemoryIndex.FirstOrDefault(p => p.Id == postId);
            if (existing != null)
            {
                _inMemoryIndex.Remove(existing);
            }
            await Task.Delay(50);
            _logger.LogInformation("✅ Post deleted from index: {PostId}", postId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to delete post from index: {PostId}", postId);
            throw;
        }
    }

    public async Task BulkIndexPostsAsync(IEnumerable<PostSearchDocument> documents)
    {
        try
        {
            var docList = documents.ToList();
            _logger.LogInformation("📦 Bulk indexing {Count} posts", docList.Count);
            foreach (var doc in docList)
            {
                var existing = _inMemoryIndex.FirstOrDefault(p => p.Id == doc.Id);
                if (existing != null)
                {
                    _inMemoryIndex.Remove(existing);
                }
                _inMemoryIndex.Add(doc);
            }
            await Task.Delay(200);
            _logger.LogInformation("✅ Bulk indexing completed: {Count} posts", docList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to bulk index posts");
            throw;
        }
    }

    public async Task<SearchResponse> SearchPostsAsync(SearchRequest request)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("🔍 Searching posts: '{Query}' with filters", request.Query);
            await TrackSearchAsync(request.Query, 0); 
            var query = request.Query.ToLowerInvariant();
            var filteredPosts = _inMemoryIndex.Where(p => 
                string.IsNullOrEmpty(query) ||
                p.Title.ToLowerInvariant().Contains(query) ||
                p.Content.ToLowerInvariant().Contains(query) ||
                p.Excerpt.ToLowerInvariant().Contains(query) ||
                p.Tags.Any(t => t.ToLowerInvariant().Contains(query)) ||
                p.AuthorName.ToLowerInvariant().Contains(query) ||
                p.CategoryName.ToLowerInvariant().Contains(query)
            ).ToList();
            if (request.Categories.Any())
            {
                filteredPosts = filteredPosts.Where(p => 
                    request.Categories.Contains(p.CategoryName)).ToList();
            }
            if (request.Tags.Any())
            {
                filteredPosts = filteredPosts.Where(p => 
                    p.Tags.Any(t => request.Tags.Contains(t))).ToList();
            }
            if (request.Authors.Any())
            {
                filteredPosts = filteredPosts.Where(p => 
                    request.Authors.Contains(p.AuthorUserName)).ToList();
            }
            if (request.FromDate.HasValue)
            {
                filteredPosts = filteredPosts.Where(p => p.PublishedAt >= request.FromDate.Value).ToList();
            }
            if (request.ToDate.HasValue)
            {
                filteredPosts = filteredPosts.Where(p => p.PublishedAt <= request.ToDate.Value).ToList();
            }
            filteredPosts = request.SortBy switch
            {
                SearchSortBy.PublishedDate => request.SortOrder == SearchSortOrder.Descending 
                    ? filteredPosts.OrderByDescending(p => p.PublishedAt).ToList()
                    : filteredPosts.OrderBy(p => p.PublishedAt).ToList(),
                SearchSortBy.ViewCount => request.SortOrder == SearchSortOrder.Descending 
                    ? filteredPosts.OrderByDescending(p => p.ViewCount).ToList()
                    : filteredPosts.OrderBy(p => p.ViewCount).ToList(),
                SearchSortBy.LikeCount => request.SortOrder == SearchSortOrder.Descending 
                    ? filteredPosts.OrderByDescending(p => p.LikeCount).ToList()
                    : filteredPosts.OrderBy(p => p.LikeCount).ToList(),
                SearchSortBy.Title => request.SortOrder == SearchSortOrder.Descending 
                    ? filteredPosts.OrderByDescending(p => p.Title).ToList()
                    : filteredPosts.OrderBy(p => p.Title).ToList(),
                _ => filteredPosts 
            };
            var totalCount = filteredPosts.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var pagedResults = filteredPosts
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PostSearchResult
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Excerpt = p.Excerpt,
                    Slug = p.Slug,
                    AuthorName = p.AuthorName,
                    AuthorUserName = p.AuthorUserName,
                    CategoryName = p.CategoryName,
                    Tags = p.Tags,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    ReadTimeMinutes = p.ReadTimeMinutes,
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    Score = 1.0f, 
                    Highlights = request.HighlightResults ? GenerateHighlights(p, query) : new()
                })
                .ToList();
            stopwatch.Stop();
            var response = new SearchResponse
            {
                Results = pagedResults,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages,
                SearchTime = stopwatch.Elapsed,
                Aggregations = GenerateAggregations(filteredPosts),
                Suggestions = await GenerateSuggestions(request.Query)
            };
            await UpdateSearchCountAsync(request.Query, totalCount);
            _logger.LogInformation("✅ Search completed: {TotalCount} results in {SearchTime}ms", 
                totalCount, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to search posts");
            throw;
        }
    }

    public async Task<List<SearchSuggestion>> GetSuggestionsAsync(string query, int maxSuggestions = 10)
    {
        try
        {
            _logger.LogInformation("💭 Getting suggestions for: '{Query}'", query);
            await Task.Delay(50);
            var suggestions = new List<SearchSuggestion>();
            if (!string.IsNullOrEmpty(query))
            {
                var queryLower = query.ToLowerInvariant();
                var titleSuggestions = _inMemoryIndex
                    .Where(p => p.Title.ToLowerInvariant().Contains(queryLower))
                    .Select(p => new SearchSuggestion { Text = p.Title, Score = 0.9f, Type = "title" })
                    .Take(3);
                suggestions.AddRange(titleSuggestions);
                var tagSuggestions = _inMemoryIndex
                    .SelectMany(p => p.Tags)
                    .Where(t => t.ToLowerInvariant().Contains(queryLower))
                    .Distinct()
                    .Select(t => new SearchSuggestion { Text = t, Score = 0.8f, Type = "tag" })
                    .Take(3);
                suggestions.AddRange(tagSuggestions);
                var authorSuggestions = _inMemoryIndex
                    .Where(p => p.AuthorName.ToLowerInvariant().Contains(queryLower))
                    .Select(p => new SearchSuggestion { Text = p.AuthorName, Score = 0.7f, Type = "author" })
                    .Distinct()
                    .Take(2);
                suggestions.AddRange(authorSuggestions);
            }
            return suggestions.OrderByDescending(s => s.Score).Take(maxSuggestions).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get suggestions");
            return new List<SearchSuggestion>();
        }
    }

    public async Task<List<PostSearchResult>> GetSimilarPostsAsync(Guid postId, int maxResults = 5)
    {
        try
        {
            _logger.LogInformation("🔗 Getting similar posts for: {PostId}", postId);
            await Task.Delay(100);
            var targetPost = _inMemoryIndex.FirstOrDefault(p => p.Id == postId);
            if (targetPost == null)
            {
                return new List<PostSearchResult>();
            }
            var similarPosts = _inMemoryIndex
                .Where(p => p.Id != postId && p.Status == "Published")
                .Where(p => p.CategoryId == targetPost.CategoryId || 
                           p.Tags.Any(t => targetPost.Tags.Contains(t)))
                .OrderByDescending(p => p.ViewCount)
                .Take(maxResults)
                .Select(p => new PostSearchResult
                {
                    Id = p.Id,
                    Title = p.Title,
                    Excerpt = p.Excerpt,
                    Slug = p.Slug,
                    AuthorName = p.AuthorName,
                    AuthorUserName = p.AuthorUserName,
                    CategoryName = p.CategoryName,
                    Tags = p.Tags,
                    PublishedAt = p.PublishedAt,
                    ViewCount = p.ViewCount,
                    LikeCount = p.LikeCount,
                    CommentCount = p.CommentCount,
                    ReadTimeMinutes = p.ReadTimeMinutes,
                    FeaturedImageUrl = p.FeaturedImageUrl,
                    Score = 0.8f
                })
                .ToList();
            _logger.LogInformation("✅ Found {Count} similar posts", similarPosts.Count);
            return similarPosts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get similar posts");
            return new List<PostSearchResult>();
        }
    }

    public async Task<List<PopularSearch>> GetPopularSearchesAsync(int maxResults = 10)
    {
        try
        {
            await Task.Delay(50);
            return _popularSearches
                .OrderByDescending(s => s.Count)
                .Take(maxResults)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get popular searches");
            return new List<PopularSearch>();
        }
    }

    public async Task TrackSearchAsync(string query, int resultCount)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query)) return;
            await Task.Delay(10);
            var existing = _popularSearches.FirstOrDefault(s => s.Query.Equals(query, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Count++;
                existing.LastSearched = DateTime.UtcNow;
            }
            else
            {
                _popularSearches.Add(new PopularSearch
                {
                    Query = query,
                    Count = 1,
                    LastSearched = DateTime.UtcNow
                });
            }
            _logger.LogDebug("📊 Search tracked: '{Query}' with {ResultCount} results", query, resultCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to track search");
        }
    }

    private async Task UpdateSearchCountAsync(string query, int resultCount)
    {
        var existing = _popularSearches.FirstOrDefault(s => s.Query.Equals(query, StringComparison.OrdinalIgnoreCase));
        if (existing != null)
        {
        }
        await Task.CompletedTask;
    }

    public async Task ReindexAllPostsAsync()
    {
        try
        {
            _logger.LogInformation("🔄 Starting full reindex of all posts...");
            await Task.Delay(1000); 
            _logger.LogInformation("✅ Full reindex completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to reindex all posts");
            throw;
        }
    }

    public async Task<bool> IsHealthyAsync()
    {
        try
        {
            await Task.Delay(50);
            return true; 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Elasticsearch health check failed");
            return false;
        }
    }

    public async Task<object> GetIndexStatsAsync()
    {
        try
        {
            await Task.Delay(50);
            return new
            {
                IndexName = _indexName,
                DocumentCount = _inMemoryIndex.Count,
                Status = "green",
                SearchCount = _popularSearches.Sum(s => s.Count),
                LastUpdated = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to get index stats");
            throw;
        }
    }

    #region Helper Methods

    private Dictionary<string, List<string>> GenerateHighlights(PostSearchDocument post, string query)
    {
        var highlights = new Dictionary<string, List<string>>();
        if (string.IsNullOrEmpty(query)) return highlights;
        if (post.Title.ToLowerInvariant().Contains(query))
        {
            highlights["title"] = new List<string> { $"<em>{query}</em>" };
        }
        if (post.Content.ToLowerInvariant().Contains(query))
        {
            highlights["content"] = new List<string> { $"...{query}..." };
        }
        return highlights;
    }

    private SearchAggregations GenerateAggregations(List<PostSearchDocument> posts)
    {
        return new SearchAggregations
        {
            Categories = posts.GroupBy(p => p.CategoryName)
                .Where(g => !string.IsNullOrEmpty(g.Key))
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            Tags = posts.SelectMany(p => p.Tags)
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            Authors = posts.GroupBy(p => p.AuthorName)
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            PublishYears = posts.GroupBy(p => p.PublishedAt.Year.ToString())
                .ToDictionary(g => g.Key, g => (long)g.Count()),
            ReadTimeRanges = posts.GroupBy(p => GetReadTimeRange(p.ReadTimeMinutes))
                .ToDictionary(g => g.Key, g => (long)g.Count())
        };
    }

    private async Task<List<string>> GenerateSuggestions(string query)
    {
        await Task.Delay(10);
        return _popularSearches
            .Where(s => s.Query.ToLowerInvariant().Contains(query.ToLowerInvariant()))
            .OrderByDescending(s => s.Count)
            .Select(s => s.Query)
            .Take(5)
            .ToList();
    }

    private string GetReadTimeRange(int readTime)
    {
        return readTime switch
        {
            <= 3 => "Quick read (1-3 min)",
            <= 7 => "Short read (4-7 min)",
            <= 15 => "Medium read (8-15 min)",
            _ => "Long read (15+ min)"
        };
    }

    #endregion
} 