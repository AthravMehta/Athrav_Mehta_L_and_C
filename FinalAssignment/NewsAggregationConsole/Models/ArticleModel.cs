namespace NewsAggregationConsole.Models
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
    }
}
