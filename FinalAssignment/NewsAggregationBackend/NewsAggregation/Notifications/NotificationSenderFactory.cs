using NewsAggregation.Notifications.Contracts;

namespace NewsAggregation.Notifications
{
    public class NotificationSenderFactory : INotificationSenderFactory
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
                NotificationType.Email => _provider.GetRequiredService<EmailNotificationSender>(),
                _ => throw new NotImplementedException()
            };
        }
    }
}