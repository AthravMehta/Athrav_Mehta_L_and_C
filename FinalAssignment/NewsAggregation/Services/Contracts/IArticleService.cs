using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IArticleService
    {
        Task<ArticleDto> AddAsync(ArticleDto dto);
        Task<ArticleDto> UpdateAsync(int id, ArticleDto dto);
        Task DeleteAsync(int id);
        Task<ArticleDto> GetByIdAsync(int id);
        Task<IEnumerable<ArticleDto>> GetAllAsync();
    }
}
