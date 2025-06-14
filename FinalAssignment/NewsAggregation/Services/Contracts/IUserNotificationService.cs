using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserNotificationService
    {
        Task<UserNotificationDto> AddAsync(UserNotificationDto dto);
        Task<UserNotificationDto> UpdateAsync(int id, UserNotificationDto dto);
        Task DeleteAsync(int id);
        Task<UserNotificationDto> GetByIdAsync(int id);
        Task<IEnumerable<UserNotificationDto>> GetAllAsync(Guid? userId = null);
    }
}
