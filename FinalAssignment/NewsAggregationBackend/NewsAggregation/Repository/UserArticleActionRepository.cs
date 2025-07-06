using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserArticleActionRepository : IUserArticleActionRepository
    {
        private readonly NewsAggregationDbContext _context;

        public UserArticleActionRepository(NewsAggregationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> AddArticleReaction(int userId, ArticleReactionRequestDto articleReactionRequestDto)
        {
            
            var existingReaction = await _context.UserArticleReactions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ArticleId == articleReactionRequestDto.ArticleId);

            if (existingReaction != null)
            {
                if (existingReaction.Reaction == articleReactionRequestDto.ArticleReaction)
                    return false;

                existingReaction.Reaction = articleReactionRequestDto.ArticleReaction;
                existingReaction.ActionCreatedTime = DateTime.UtcNow;
                _context.UserArticleReactions.Update(existingReaction);
            }
            else
            {
                var newReaction = new UserArticleReaction
                {
                    UserId = userId,
                    ArticleId = articleReactionRequestDto.ArticleId,
                    Reaction = articleReactionRequestDto.ArticleReaction,
                    ActionCreatedTime = DateTime.UtcNow
                };
                await _context.UserArticleReactions.AddAsync(newReaction);
            }
            return true;
        }

        public async Task<bool> DeleteArticleReaction(int userId, int articleId)
        {
            var existingReaction = await _context.UserArticleReactions
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ArticleId == articleId);

            if (existingReaction == null)
                return false;

            _context.UserArticleReactions.Remove(existingReaction);
            return true;
        }


        public async Task<ToggleSaveResponseDto> ToggleSaveAsync(int userId, int articleId)
        {
            var existing = await _context.UserSavedArticles
                .FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

            if (existing != null)
            {
                _context.UserSavedArticles.Remove(existing);
                return new ToggleSaveResponseDto
                {
                    IsSaved = false,
                    Message = SuccessConstants.ArticleUnsaved
                };
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
                return new ToggleSaveResponseDto
                {
                    IsSaved = true,
                    Message = SuccessConstants.ArticleSaved
                };
            }
        }

        public async Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync(int userId)
        {
            return await _context.UserSavedArticles
                .Where(x => x.UserId == userId)
                .Select(x => x.ArticleId)
                .ToListAsync();
        }

        public async Task<bool> HasUserReportedArticleAsync(int articleId, int userId)
        => await _context.UserArticleReports
            .AnyAsync(r => r.ArticleId == articleId && r.UserId == userId);

        public async Task AddReportAsync(UserArticleReport report)
        {
            _context.UserArticleReports.Add(report);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetReportCountForArticleAsync(int articleId) {
            return await _context.UserArticleReports
                .CountAsync(r => r.ArticleId == articleId);
        }

        public async Task<List<int>> GetCategoriesByUserReactionAsync(int userId, ReactionEnum reaction)
        {
            return await _context.UserArticleReactions
                .Where(r => r.UserId == userId && r.Reaction == reaction)
                .Select(r => r.Article.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<int>> GetCategoriesBySavedUserArticleAsync(int userId)
        {
            return await _context.UserSavedArticles
                .Where(s => s.UserId == userId)
                .Select(s => s.Article.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<int>> GetReportedArticleCategoriesByUserAsync(int userId)
        {
            return await _context.UserArticleReports
                .Where(r => r.UserId == userId)
                .Select(r => r.Article.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}