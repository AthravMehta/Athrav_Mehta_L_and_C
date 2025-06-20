using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserArticleActionService
    {
        Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync();
        Task<ToggleSaveResponseDto> ToggleSaveAsync(int articleId);
        Task<bool> AddArticleReaction(ArticleReactionRequestDto articleReactionRequestDto);
        Task<bool> DeleteArticleReaction(int articleId);
    }

}
