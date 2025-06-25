using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Notifications;
using NewsAggregation.Constants;

namespace NewsAggregation.Notifications
{
    public class NotificationSenderFactory
    {
        private readonly IServiceProvider _provider;

        public NotificationSenderFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public INotificationSender GetSender(string type)
        {
            return type switch
            {
                AppConstants.Email => _provider.GetRequiredService<EmailNotificationSender>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}