using System.ComponentModel.DataAnnotations;

namespace NewsAggregationConsole.Models
{
    public class UserArticleReportDto
    {
        public int? UserArticleReportId { get; set; }

        [MaxLength(500)]
        public string ReportReason { get; set; }
        public DateTime ActionCreatedTime { get; set; }

        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
    public class UserArticleReportResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
