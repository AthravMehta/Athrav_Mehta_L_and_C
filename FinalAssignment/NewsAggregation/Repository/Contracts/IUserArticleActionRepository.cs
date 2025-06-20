using Microsoft.EntityFrameworkCore;
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

    }

}
