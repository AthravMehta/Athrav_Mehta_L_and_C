using NewsAggregation.Entities;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserNotificationRepository
    {
        Task AddRangeAsync(IEnumerable<UserNotification> notifications);
        Task<IEnumerable<UserNotification>> GetAllUserNotificationAsync(int? UserId = null);
        /// <summary>
        /// This marks all the user received notification as read.
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        Task MarkAllUserNotificationsAsRead(int? UserId = null);

        /// <summary>
        /// Get the category id's 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<int>> GetCategoriesByUserNotificationsAsync(int userId);

        /// <summary>
        /// Gets all user keywords
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UserKeyword>> GetKeywordsByUserNotificationsAsync(int userId);
    }

}
