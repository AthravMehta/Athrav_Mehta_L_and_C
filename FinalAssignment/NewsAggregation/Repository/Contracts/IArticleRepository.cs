using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IArticleRepository
    {
        Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate);
    }

}
