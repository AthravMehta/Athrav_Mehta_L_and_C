using NewsAggregation.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewsAggregation.Entities
{
    public class UserArticleReport
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserArticleReportId { get; set; }

        [MaxLength(500)]
        public string ReportReason { get; set; }
        public DateTime ActionCreatedTime { get; set; }

        public int UserId { get; set; }
        public int ArticleId { get; set; }

        [JsonIgnore]
        public User User { get; set; }

        [JsonIgnore]
        public Article Article { get; set; }
    }
}
