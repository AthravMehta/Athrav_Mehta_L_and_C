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
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddRangeAsync(IEnumerable<UserNotification> notifications)
        {
            if (notifications == null)
                throw new ArgumentNullException(nameof(notifications));

            await _dbContext.UserNotifications.AddRangeAsync(notifications);
        }

        public async Task<IEnumerable<UserNotification>> GetAllUserNotificationAsync(int? userId = null)
        {
            if (!userId.HasValue)
                return Enumerable.Empty<UserNotification>();

            var notifications = await _dbContext.UserNotifications
                .Where(un => un.UserId == userId.Value && !un.IsRead)
                .Include(un => un.Article)
                .ToListAsync();

            return notifications ?? Enumerable.Empty<UserNotification>();
        }

        public async Task MarkAllUserNotificationsAsRead(int? userId = null)
        {
            if (!userId.HasValue)
                return;

            var notifications = await _dbContext.UserNotifications
                .Where(n => n.UserId == userId.Value && !n.IsRead)
                .ToListAsync();

            if (notifications.Any())
            {
                foreach (var notification in notifications)
                {
                    notification.IsRead = true;
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<int>> GetCategoriesByUserNotificationsAsync(int userId)
        {
            return await _dbContext.UserNotificationConfigurations
                .Where(n => n.UserId == userId && n.IsEnabled)
                .Select(n => n.CategoryId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<UserKeyword>> GetKeywordsByUserNotificationsAsync(int userId)
        {
            return await _dbContext.UserKeywords
                .Where(k => k.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
