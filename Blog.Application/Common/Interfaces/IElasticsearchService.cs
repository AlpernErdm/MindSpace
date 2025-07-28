using Blog.Application.Common.Search;

namespace Blog.Application.Common.Interfaces;

public interface IElasticsearchService
{
    Task InitializeAsync();
    Task IndexPostAsync(PostSearchDocument document);
    Task UpdatePostAsync(PostSearchDocument document);
    Task DeletePostAsync(Guid postId);
    Task BulkIndexPostsAsync(IEnumerable<PostSearchDocument> documents);
    Task<SearchResponse> SearchPostsAsync(SearchRequest request);
    Task<List<SearchSuggestion>> GetSuggestionsAsync(string query, int maxSuggestions = 10);
    Task<List<PostSearchResult>> GetSimilarPostsAsync(Guid postId, int maxResults = 5);
    Task<List<PopularSearch>> GetPopularSearchesAsync(int maxResults = 10);
    Task TrackSearchAsync(string query, int resultCount);
    Task ReindexAllPostsAsync();
    Task<bool> IsHealthyAsync();
    Task<object> GetIndexStatsAsync();
} 