namespace NewsAggregationConsole.Models
{
    public class CategoryDto
    {
        public int? CategoryId { get; set; }
        public string Name { get; set; }
        public bool? IsHidden { get; set; }
        public string? HideReason { get; set; }

        public DateTime? CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }

    public class CategoryWithKeywordsDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } 
        public bool IsHidden { get; set; }
        public string HideReason { get; set; } = string.Empty;
        public List<KeywordDto> Keywords { get; set; } = new List<KeywordDto>();
    }
}
