using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleActionRepository
    {
        /// <summary>
        /// Save or Unsave any article
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        Task<ToggleSaveResponseDto> ToggleSaveAsync(int userId, int articleId);

        /// <summary>
        /// Like or Dislike any article
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleReactionRequestDto"></param>
        /// <returns></returns>
        Task<bool> AddArticleReaction(int userId, ArticleReactionRequestDto articleReactionRequestDto);

        /// <summary>
        /// Deletes any article reaction
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="articleId"></param>
        /// <returns></returns>
        Task<bool> DeleteArticleReaction(int userId, int articleId);

        /// <summary>
        /// get all articles Id's which are saved for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync(int userId);

        /// <summary>
        /// Checks if user has reported the article or not
        /// </summary>
        /// <param name="articleId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> HasUserReportedArticleAsync(int articleId, int userId);
        Task AddReportAsync(UserArticleReport report);

        /// <summary>
        /// Gets the report count for a article
        /// </summary>
        /// <param name="articleId"></param>
        /// <returns></returns>
        Task<int> GetReportCountForArticleAsync(int articleId);

        /// <summary>
        /// Return all the category Id's whose article user has reacted
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reaction"></param>
        /// <returns></returns>
        Task<List<int>> GetCategoriesByUserReactionAsync(int userId, ReactionEnum reaction);

        /// <summary>
        /// Return all the category Id's whose article user has saved
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<int>> GetCategoriesBySavedUserArticleAsync(int userId);

        /// <summary>
        /// Return all the category Id's whose article user has reported
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<int>> GetReportedArticleCategoriesByUserAsync(int userId);
        Task SaveChangesAsync();

    }

}
