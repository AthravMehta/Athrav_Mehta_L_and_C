namespace NewsAggregation.Notifications.Contracts
{
    public interface INotificationSender
    {
        Task SendAsync(string to, string subject, string message);
    }

}
