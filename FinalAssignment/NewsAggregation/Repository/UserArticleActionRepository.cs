using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserArticleActionRepository : IUserArticleActionRepository
    {
        private readonly NewsAggregationDbContext _context;

        public UserArticleActionRepository(NewsAggregationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ToggleSaveAsync(int userId, int articleId)
        {
            var existing = await _context.UserSavedArticles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (existing != null)
            {
                _context.UserSavedArticles.Remove(existing);
                await _context.SaveChangesAsync();
                return false;
            }
            else
            {
                var action = new UserSavedArticle
                {
                    UserId = userId,
                    ArticleId = articleId,
                    ActionCreatedTime = DateTime.UtcNow
                };
                _context.UserSavedArticles.Add(action);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }
}