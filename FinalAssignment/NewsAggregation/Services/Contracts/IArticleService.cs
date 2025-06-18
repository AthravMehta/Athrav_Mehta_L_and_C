using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IArticleService
    {
        Task<ArticleDto> AddAsync(ArticleDto dto);
        Task<ArticleDto> GetByIdAsync(int id);
        Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate);
    }
}
