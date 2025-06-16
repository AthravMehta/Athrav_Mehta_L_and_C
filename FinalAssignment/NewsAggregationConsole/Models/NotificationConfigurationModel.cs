namespace NewsAggregationConsole.Models
{
    public class NotificationConfigurationDto
    {
        public int? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsEnabled { get; set; }
    }
}
