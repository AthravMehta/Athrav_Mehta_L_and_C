using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleActionRepository
    {
        Task<ToggleSaveResponseDto> ToggleSaveAsync(int userId, int articleId);
        Task<bool> AddArticleReaction(int userId, ArticleReactionRequestDto articleReactionRequestDto);
        Task<bool> DeleteArticleReaction(int userId, int articleId);
        Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync(int userId);
        Task SaveChangesAsync();
        Task<bool> HasUserReportedArticleAsync(int articleId, int userId);
        Task AddReportAsync(UserArticleReport report);
        Task<int> GetReportCountForArticleAsync(int articleId);
        Task<List<int>> GetCategoriesByUserReactionAsync(int userId, ReactionEnum reaction);
        Task<List<int>> GetCategoriesBySavedUserArticleAsync(int userId);
        Task<List<int>> GetReportedArticleCategoriesByUserAsync(int userId);

    }

}
