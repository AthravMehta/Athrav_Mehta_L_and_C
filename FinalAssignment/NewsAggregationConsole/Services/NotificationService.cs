using NewsAggregationConsole.Models;

namespace NewsAggregationConsole.Services
{
    public class NotificationService
    {
        private readonly ApiService _apiService;

        public NotificationService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<NotificationDto>> GetUnviewedNotificationsAsync()
        {
            return await _apiService.GetAsync<List<NotificationDto>>($"/api/usernotification");
        }
    }
}
