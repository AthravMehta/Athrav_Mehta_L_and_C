using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IArticleRepository
    {
        Task<IEnumerable<Article>> GetAllAsync(ArticleQueryDto articleQueryDto);
        Task<ArticleDetailsDto> GetArticleWithUserStatusAsync(int articleId);
        Task<Article> GetArticleByIdAsync(int articleId);

        /// <summary>
        /// To get multiple Articles together through Id's
        /// </summary>
        /// <param name="articleIds"></param>
        /// <returns></returns>
        Task<IEnumerable<Article>> GetArticlesByIdsAsync(IEnumerable<int> articleIds);
        Task AddRangeAsync(IEnumerable<Article> articlesToAdd);

        /// <summary>
        /// Checks if article exist in db or not, throught title, content.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>
        Task<bool> ArticleExistsAsync(Article article);
        Task<bool> UpdateArticle(Article article);
    }

}
