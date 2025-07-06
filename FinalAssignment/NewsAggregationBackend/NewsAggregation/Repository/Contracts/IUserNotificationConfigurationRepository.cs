using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserNotificationConfigurationRepository
    {
        /// <summary>
        /// Gets all the user notification configuration
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserNotificationConfiguration>> GetAllUserConfigurationAsync();

        /// <summary>
        /// Checks if notification configuration already exist or not
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(int userId, int categoryId);
    }
}
