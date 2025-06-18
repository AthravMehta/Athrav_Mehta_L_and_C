using NewsAggregation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserNotificationConfigurationService
    {
        Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto);
        Task<UserNotificationConfigurationDto> UpdateAsync(int id, UserNotificationConfigurationDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<UserNotificationConfigurationDto>> GetAllAsync();
    }
}
