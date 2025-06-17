using NewsAggregationConsole.Models;
using NewsAggregationConsole.Services;

public class ArticleService
{
    private readonly ApiService _apiService;

    public ArticleService(ApiService apiService)
    {
        _apiService = apiService;
    }

    public async Task<List<ArticleDto>> GetArticlesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        string url = $"/api/article?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
        return await _apiService.GetAsync<List<ArticleDto>>(url);
    }

    public async Task<bool> SaveArticleForUserAsync(int articleId)
    {
        var result = await _apiService.PostAsync<bool>("/api/article/toggle-save", new { ArticleId = articleId });
        return result;
    }
}
