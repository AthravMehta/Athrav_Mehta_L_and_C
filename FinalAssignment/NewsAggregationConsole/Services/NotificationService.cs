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

        public async Task<List<NotificationDto>> GetUnviewedNotificationsAsync(int userId)
        {
            return await _apiService.GetAsync<List<NotificationDto>>($"/api/usernotification?userId={userId}");
        }

        //public async Task MarkNotificationsAsViewedAsync(List<int> notificationIds)
        //{
        //    await _apiService.PostAsync<object>("/api/notifications/mark-viewed", notificationIds);
        //}

        //public async Task<NotificationConfigDto> GetNotificationConfigAsync(int userId)
        //{
        //    return await _apiService.GetAsync<NotificationConfigDto>($"/api/notifications/config/{userId}");
        //}

        //public async Task ToggleCategoryAsync(int userId, string categoryName)
        //{
        //    await _apiService.PostAsync<object>($"/api/notifications/config/toggle-category", new { userId, categoryName });
        //}

        //public async Task AddKeywordAsync(int userId, string keyword)
        //{
        //    await _apiService.PostAsync<object>($"/api/notifications/config/add-keyword", new { userId, keyword });
        //}
    }
}
