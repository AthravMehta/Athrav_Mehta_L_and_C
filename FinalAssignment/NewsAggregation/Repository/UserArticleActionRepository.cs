using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Enums;

public class UserArticleActionRepository : IUserArticleActionRepository
{
    private readonly NewsAggregationDbContext _context;

    public UserArticleActionRepository(NewsAggregationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ToggleSaveAsync(Guid userId, int articleId)
    {
        var existing = await _context.UserArticleActions
            .FirstOrDefaultAsync(x => x.UserId == userId && x.ArticleId == articleId);

        if (existing != null)
        {
            _context.UserArticleActions.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }
        else
        {
            var action = new UserArticleAction
            {
                UserId = userId,
                ArticleId = articleId,
                ArticleAction = ArticleActionEnum.Saved,
                ActionCreatedTime = DateTime.UtcNow
            };
            _context.UserArticleActions.Add(action);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
