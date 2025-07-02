using NewsAggregation.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Entities
{
    public class UserArticleReaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserArticleReactionId { get; set; }
        public ReactionEnum Reaction { get; set; }
        public DateTime ActionCreatedTime { get; set; }

        public int UserId { get; set; }
        public int ArticleId { get; set; }
        public User User { get; set; }
        public Article Article { get; set; }
    }
}
