namespace NewsAggregationConsole.Models
{
    public class NotificationDto
    {
        public int UserNotificationId { get; set; }
        public int UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool IsRead { get; set; }
        public ArticleDto articleDto { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
