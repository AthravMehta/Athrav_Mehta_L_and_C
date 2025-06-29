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
    }
}
