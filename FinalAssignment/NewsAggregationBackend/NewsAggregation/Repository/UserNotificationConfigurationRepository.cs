using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserNotificationConfigurationRepository : IUserNotificationConfigurationRepository
    {
        private readonly NewsAggregationDbContext _context;
        private readonly RequestContext _requestContext;

        public UserNotificationConfigurationRepository(NewsAggregationDbContext context, RequestContext requestContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        public async Task<IEnumerable<UserNotificationConfiguration>> GetAllUserConfigurationAsync()
        {
            var userId = _requestContext.UserId;

            if (!userId.HasValue)
            {
                return Enumerable.Empty<UserNotificationConfiguration>();
            }

            var configs = await _context.UserNotificationConfigurations
                .Where(config => config.UserId == userId)
                .Include(config => config.Category)
                .ToListAsync();

            return configs ?? Enumerable.Empty<UserNotificationConfiguration>();
        }

        public async Task<bool> ExistsAsync(int userId, int categoryId)
        {
            return await _context.UserNotificationConfigurations
                .AnyAsync(x => x.UserId == userId && x.CategoryId == categoryId);
        }
    }
}
