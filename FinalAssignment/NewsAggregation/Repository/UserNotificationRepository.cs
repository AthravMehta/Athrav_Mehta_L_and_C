using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserNotificationRepository : IUserNotificationRepository
    {
        private readonly NewsAggregationDbContext _dbContext;

        public UserNotificationRepository(NewsAggregationDbContext context)
        {
            _dbContext = context;
        }

        public async Task AddRangeAsync(IEnumerable<UserNotification> notifications)
        {
            await _dbContext.UserNotifications.AddRangeAsync(notifications);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserNotification>> GetAllUserNotificationAsync(int? userId = null)
        {
            if (userId == null)
                return Enumerable.Empty<UserNotification>();

            return await _dbContext.UserNotifications
                .Where(un => un.UserId == userId)
                .ToListAsync();
        }
    }
}
