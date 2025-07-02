using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;
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

    }

}
