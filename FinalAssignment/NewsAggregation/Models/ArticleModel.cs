namespace NewsAggregation.Models
{
    public class ArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int ExternalServerId { get; set; }
        public int CategoryId { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

    }

    public class ToggleSaveRequestDto
    {
        public int ArticleId { get; set; }
    }
    public class ArticleReactionRequestDto
    {
        public int ArticleId { get; set; }
        public int ArticleReaction { get; set; }
    }
}
