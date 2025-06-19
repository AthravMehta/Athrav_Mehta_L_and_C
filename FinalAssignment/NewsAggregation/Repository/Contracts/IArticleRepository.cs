using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate);
        Task AddRangeAsync(IEnumerable<Article> articlesToAdd);
        Task<bool> ArticleExistsAsync(Article article);
    }

}
