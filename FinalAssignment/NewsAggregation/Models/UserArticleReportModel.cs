using NewsAggregation.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Models
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
}
