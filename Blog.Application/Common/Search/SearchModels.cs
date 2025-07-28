namespace Blog.Application.Common.Search;

/// <summary>
/// Elasticsearch post search document
/// </summary>
public class PostSearchDocument
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUserName { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ReadTimeMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    public string? FeaturedImageUrl { get; set; }
}

/// <summary>
/// Search request model
/// </summary>
public class SearchRequest
{
    public string Query { get; set; } = string.Empty;
    public List<string> Categories { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> Authors { get; set; } = new();
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int MinReadTime { get; set; } = 0;
    public int MaxReadTime { get; set; } = 999;
    public SearchSortBy SortBy { get; set; } = SearchSortBy.Relevance;
    public SearchSortOrder SortOrder { get; set; } = SearchSortOrder.Descending;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool HighlightResults { get; set; } = true;
}

/// <summary>
/// Search response model
/// </summary>
public class SearchResponse
{
    public List<PostSearchResult> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public TimeSpan SearchTime { get; set; }
    public SearchAggregations? Aggregations { get; set; }
    public List<string> Suggestions { get; set; } = new();
}

/// <summary>
/// Individual search result
/// </summary>
public class PostSearchResult
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Excerpt { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorUserName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime PublishedAt { get; set; }
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public int ReadTimeMinutes { get; set; }
    public string? FeaturedImageUrl { get; set; }
    public float Score { get; set; }
    public Dictionary<string, List<string>> Highlights { get; set; } = new();
}

/// <summary>
/// Search aggregations (facets)
/// </summary>
public class SearchAggregations
{
    public Dictionary<string, long> Categories { get; set; } = new();
    public Dictionary<string, long> Tags { get; set; } = new();
    public Dictionary<string, long> Authors { get; set; } = new();
    public Dictionary<string, long> PublishYears { get; set; } = new();
    public Dictionary<string, long> ReadTimeRanges { get; set; } = new();
}

/// <summary>
/// Auto-complete suggestion
/// </summary>
public class SearchSuggestion
{
    public string Text { get; set; } = string.Empty;
    public float Score { get; set; }
    public string Type { get; set; } = string.Empty; // title, tag, author, category
}

/// <summary>
/// Search sort options
/// </summary>
public enum SearchSortBy
{
    Relevance,
    PublishedDate,
    CreatedDate,
    UpdatedDate,
    ViewCount,
    LikeCount,
    CommentCount,
    ReadTime,
    Title,
    AuthorName
}

/// <summary>
/// Search sort order
/// </summary>
public enum SearchSortOrder
{
    Ascending,
    Descending
}

/// <summary>
/// Popular search analytics
/// </summary>
public class PopularSearch
{
    public string Query { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime LastSearched { get; set; }
} 