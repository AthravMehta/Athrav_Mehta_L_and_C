using NewsAggregation.Configurations;
using NewsAggregation.Entities;

namespace NewsAggregation.NewsAggregation.Entities
{
    public class UserArticleReadTracking : BaseAuditEntity
    {
        public int UserArticleReadTrackingId { get; set; }
        public int UserId { get; set; }
        public int ArticleId { get; set; }
        public int CategoryId { get; set; }

        public User User { get; set; }
        public Article Article { get; set; }
        public Category Category { get; set; }
    }
}
