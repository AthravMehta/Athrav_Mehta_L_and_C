namespace NewsAggregationConsole.Models
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public Guid ArticleId { get; set; }
        public DateTime DateSent { get; set; }
        public bool IsRead { get; set; }
    }
}
