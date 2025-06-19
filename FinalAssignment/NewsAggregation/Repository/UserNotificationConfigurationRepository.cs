using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserNotificationConfigurationRepository : IUserNotificationConfigurationRepository
    {
        private readonly NewsAggregationDbContext _context;
        private readonly RequestContext _requestContext;

        public UserNotificationConfigurationRepository(NewsAggregationDbContext context, RequestContext requestContext)
        {
            _context = context;
            _requestContext = requestContext;
        }

        public async Task<IEnumerable<UserNotificationConfiguration>> GetAllUserConfigurationAsync()
        {
            var userId = _requestContext.UserId;

            if (userId == null)
            {
                return Enumerable.Empty<UserNotificationConfiguration>();
            }

            return await _context.UserNotificationConfigurations
                .Where(config => config.UserId == userId)
                .ToListAsync();
        }
        public async Task<bool> ExistsAsync(int userId, int categoryId)
        {
            return await _context.UserNotificationConfigurations
                .AnyAsync(x => x.UserId == userId && x.CategoryId == categoryId);
        }

    }
}
