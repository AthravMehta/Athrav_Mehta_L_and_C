using NewsAggregation.Enums;

namespace NewsAggregation.Models
{
    public class ArticleDto
    {
        public int ArticleId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Source { get; set; }
        public string Url { get; set; }
        public DateTime PublishedDate { get; set; }
        public int ExternalServerId { get; set; }
        public int CategoryId { get; set; }
        public int UserArticleReactions { get; set; }   
        public int UserSavedArticles { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
    }
    public class ArticleQueryDto
    {
        public string SearchText { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? CategoryId { get; set; }
        public bool SortByLikes { get; set; }
        public bool SortByDislikes { get; set; }
    }

    public class ToggleSaveRequestDto
    {
        public int ArticleId { get; set; }
    }

    public class ToggleSaveResponseDto
    {
        public bool IsSaved { get; set; }
        public string Message { get; set; }
    }
    public class ArticleReactionRequestDto
    {
        public int ArticleId { get; set; }
        public ReactionEnum ArticleReaction { get; set; }
    }
}
