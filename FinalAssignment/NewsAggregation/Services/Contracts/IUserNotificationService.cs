using NewsAggregation.Entities;
using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserNotificationService
    {
        Task<UserNotificationDto> AddAsync(UserNotificationDto userNotificationDto);
        Task AddRangeAsync(IEnumerable<UserNotification> notifications);
        Task<UserNotificationDto> UpdateAsync(int id, UserNotificationDto userNotificationDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserNotificationDto>> GetAllUserNotificationAsync();
    }
}
