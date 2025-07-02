using NewsAggregation.Entities;

namespace NewsAggregation.Models
{
    public class UserSavedArticleDto
    {
        public int UserSavedArticleId { get; set; }
        public DateTime ActionCreatedTime { get; set; }

        public int UserId { get; set; }
        public int ArticleId { get; set; }
    }
}
