namespace NewsAggregation.Models
{
    public class UserKeywordDto
    {
        public int? Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}
