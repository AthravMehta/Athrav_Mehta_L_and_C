using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IArticleService
    {
        Task<ArticleDto> AddAsync(ArticleDto dto);
        Task<IEnumerable<Article>> AddAllArticlesAsync(IEnumerable<Article> articles);
        Task<ArticleDetailsDto> GetByIdWithUserDetailsAsync(int id);
        Task<IEnumerable<ArticleDto>> GetAllAsync(ArticleQueryDto articleQueryDto);
        Task<IEnumerable<ArticleDto>> GetSavedArticlesForCurrentUserAsync();
        Task<bool> HideArticleAsync(int articleId);

        /// <summary>
        /// This method will Hide Article of Particular Category passed.
        /// To Make this method work, you must pass your desired category and IsHidden Status in query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task HideArticlesByCategoryAsync(ArticleQueryDto query);

        /// <summary>
        /// This method will Unhide Article of Particular Category passed.
        /// To Make this method work, you must pass your desired category and IsHidden Status in query.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task UnhideArticlesByCategoryAsync(ArticleQueryDto query);

        /// <summary>
        /// This method will Hide Article of Particular Keyword passed.
        /// </summary>
        /// <param name="keywordId"></param>
        /// <returns></returns>
        Task HideArticlesByKeywordAsync(int keywordId);

        /// <summary>
        /// This method will Unhide Article of Particular Keyword passed.
        /// </summary>
        /// <param name="keywordId"></param>
        /// <returns></returns>
        Task UnhideArticlesByKeywordAsync(int keywordId);
    }
}
