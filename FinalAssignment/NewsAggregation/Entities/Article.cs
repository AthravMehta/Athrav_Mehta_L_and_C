using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Entities
{
    public class Article : BaseAuditEntity, BaseKeyEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public Guid ExternalServerId { get; set; }
        public ICollection<UserArticleAction> UserArticleActions { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime PublishedDate { get; set; }
    }

}
