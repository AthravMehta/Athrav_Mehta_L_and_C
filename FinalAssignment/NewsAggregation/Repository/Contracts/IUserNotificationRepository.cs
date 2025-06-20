using NewsAggregation.Entities;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserNotificationRepository
    {
        Task AddRangeAsync(IEnumerable<UserNotification> notifications);
        Task<IEnumerable<UserNotification>> GetAllUserNotificationAsync(int? UserId = null);
    }

}
