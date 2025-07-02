using NewsAggregation.Enums;

namespace NewsAggregation.Models
{
    public class UserArticleReactionDto
    {
        public int UserArticleReactionId { get; set; }
        public ReactionEnum Reaction { get; set; }
        public DateTime ActionCreatedTime { get; set; }
        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
}
