using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NewsAggregation.Enums;
using System.Text.Json.Serialization;

namespace NewsAggregation.Entities
{
    public class Article : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ArticleId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
        public bool IsHidden { get; set; } = false;
        public HideReasonEnum HideReason { get; set; } = HideReasonEnum.NotHidden;

        [ForeignKey(nameof(ExternalServerId))]
        public int ExternalServerId { get; set; }

        [JsonIgnore]
        public ICollection<UserArticleReaction> UserArticleReactions { get; set; } = new List<UserArticleReaction>();

        [JsonIgnore]
        public ICollection<UserArticleReport> UserArticleReports { get; set; } = new List<UserArticleReport>();

        [JsonIgnore]
        public ICollection<UserSavedArticle> UserSavedArticles { get; set; } = new List<UserSavedArticle>();
    }

}
