namespace NewsAggregation.Models
{
    public class UserNotificationDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ArticleId { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool IsRead { get; set; }
    }
}
