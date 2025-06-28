using NewsAggregationConsole.Helpers;
using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

public class ArticleService
{
    private readonly ApiService _apiService;

    public ArticleService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<ArticleDto>> GetFilteredArticlesAsync(ArticleQueryDto query)
    {
        string queryString = QueryStringHelper.ToQueryString(query);
        string url = $"/api/article{(string.IsNullOrEmpty(queryString) ? "" : "?" + queryString)}";
        return await _apiService.GetAsync<List<ArticleDto>>(url);
    }

    public async Task<List<ArticleDto>> GetSavedArticleForUserAsync()
    {
        string url = "/api/article/saved";
        return await _apiService.GetAsync<List<ArticleDto>>(url);
    }

    public async Task<ToggleSaveResponseDto> SaveArticleForUserAsync(int articleId)
    {
        return await _apiService.PostAsync<ToggleSaveResponseDto>("/api/article/toggle-save", new { ArticleId = articleId });
    }
    public async Task<bool> AddArticleReactionAsync(ArticleReactionRequestDto articleReactionRequestDto)
    {
        var result = await _apiService.PostAsync<bool>("/api/article/reaction", articleReactionRequestDto);
        return result;
    }
    
    public async Task<UserArticleReportResponseDto> ReportArticleAsync(int articleId, string reportReason)
    {
        var articleReportDto = new UserArticleReportDto
        {
            ArticleId = articleId,
            ReportReason = reportReason
        };
        UserArticleReportResponseDto result = await _apiService.PostAsync<UserArticleReportResponseDto>($"/api/article/{articleId}/report", articleReportDto);
        return result;
    }
}
