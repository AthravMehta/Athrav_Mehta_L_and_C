namespace NewsAggregation.Models
{
    public class KeywordDto
    {
        public int? KeywordId { get; set; }
        public string Keyword { get; set; }
        public int CategoryId { get; set; }
        public bool? IsHidden { get; set; }
        public string? HideReason { get; set; }
    }
}
