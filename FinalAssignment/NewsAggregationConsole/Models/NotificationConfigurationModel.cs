namespace NewsAggregationConsole.Models
{
    public class NotificationConfigurationDto
    {
        public int? UserNotificationConfigurationId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public bool IsEnabled { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
