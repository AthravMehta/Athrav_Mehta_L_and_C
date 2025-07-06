namespace NewsAggregation.Notifications.Contracts
{
    public interface INotificationSenderFactory
    {
        /// <summary>
        /// Get the required service email/any
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        INotificationSender GetSender(string type);
    }
}
