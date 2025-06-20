namespace NewsAggregationConsole.Models
{
    public class UserKeywordDto
    {
        public int? UserKeywordId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public bool IsEnabled { get; set; } = true;

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
