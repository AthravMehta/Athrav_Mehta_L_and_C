using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync(ArticleQueryDto articleQueryDto);
        Task<IEnumerable<Article>> GetArticlesByIdsAsync(IEnumerable<int> articleIds);
        Task AddRangeAsync(IEnumerable<Article> articlesToAdd);
        Task<bool> ArticleExistsAsync(Article article);
    }

}
