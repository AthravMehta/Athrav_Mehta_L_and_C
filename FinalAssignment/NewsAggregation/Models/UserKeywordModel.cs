namespace NewsAggregation.Models
{
    public class UserKeywordDto
    {
        public int? Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CategoryId { get; set; }
        public string Keyword { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
