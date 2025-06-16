namespace NewsAggregationConsole.Models
{
    public class NotificationDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
