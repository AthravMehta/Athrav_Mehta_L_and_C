using NewsAggregationConsole.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAggregationConsole.Services
{
    public class NotificationConfigurationService
    {
        private readonly ApiService _apiService;

        public NotificationConfigurationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<NotificationConfigurationDto>> GetUserNotificationConfigAsync()
        {
            return await _apiService.GetAsync<List<NotificationConfigurationDto>>($"/api/usernotificationconfiguration");
        }

        public async Task<NotificationConfigurationDto> ToggleCategoryAsync(int? userNotificationConfigurationId, NotificationConfigurationDto notificationConfigurationDto)
        {
            var result = await _apiService.PutAsync<NotificationConfigurationDto>($"/api/UserNotificationConfiguration/{userNotificationConfigurationId}", notificationConfigurationDto);
            return result;
        }

        public async Task AddKeywordAsync(UserKeywordDto userKeywordDto)
        {
            await _apiService.PostAsync<object>($"/api/userkeyword", userKeywordDto);
        }
    }

}
