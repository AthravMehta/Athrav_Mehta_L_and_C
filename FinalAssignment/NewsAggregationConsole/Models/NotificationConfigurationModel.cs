namespace NewsAggregationConsole.Models
{
    public class NotificationConfigurationDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public bool IsEnabled { get; set; }
    }
}
