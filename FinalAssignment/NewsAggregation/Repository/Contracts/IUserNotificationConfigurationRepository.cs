using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserNotificationConfigurationRepository
    {
        Task<IEnumerable<UserNotificationConfiguration>> GetAllUserConfigurationAsync();
        Task<bool> ExistsAsync(int userId, int categoryId);
    }
}
