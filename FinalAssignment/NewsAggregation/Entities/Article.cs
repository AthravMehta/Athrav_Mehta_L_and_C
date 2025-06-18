using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

        [ForeignKey(nameof(ExternalServerId))]
        public int ExternalServerId { get; set; }
        public ICollection<UserArticleReaction> UserArticleReactions { get; set; } = new List<UserArticleReaction>();
        public ICollection<UserSavedArticle> UserSavedArticles { get; set; } = new List<UserSavedArticle>();
        public int CategoryId { get; set; }
    }

}
