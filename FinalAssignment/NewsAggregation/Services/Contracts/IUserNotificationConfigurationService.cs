using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserNotificationConfigurationService
    {
        Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto);
        Task<UserNotificationConfigurationDto> UpdateAsync(int id, UserNotificationConfigurationDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserNotificationConfigurationDto>> GetAllConfigurationAsync();
        Task<IEnumerable<UserNotificationConfigurationDto>> GetAllUserConfigurationAsync();
        Task<bool> ExistsAsync(int userId, int categoryId);
        Task SaveChangesAsync();

        /// <summary>
        /// Initializes UserNotificationConfiguration for all existing users and categories,
        /// creating missing configurations with IsEnabled = true.
        /// </summary>
        Task InitializeNotificationConfigurationsAsync();
    }
}
