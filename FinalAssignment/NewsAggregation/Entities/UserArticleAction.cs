using NewsAggregation.Configurations;
using NewsAggregation.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Entities
{
    public class UserArticleAction : BaseKeyEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Article))]
        public int ArticleId { get; set; }
        public Article? Article { get; set; }
        public ArticleActionEnum ArticleAction { get; set; }
        public DateTime ActionCreatedTime { get; set; }
    }
}
