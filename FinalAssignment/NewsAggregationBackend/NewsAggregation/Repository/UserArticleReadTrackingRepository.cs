using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserArticleReadTrackingRepository : IUserArticleReadTrackingRepository
    {
        private readonly NewsAggregationDbContext _dbContext;

        public UserArticleReadTrackingRepository(NewsAggregationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<int>> GetCategoriesByUserReadSequenceAsync(int userId)
        {
            return await _dbContext.UserArticleReadTrackings
                .Where(r => r.UserId == userId)
                .GroupBy(r => r.CategoryId)
                .OrderByDescending(g => g.Max(r => r.CreatedDateTime))
                .Select(g => g.Key)
                .ToListAsync();
        }
    }
}
