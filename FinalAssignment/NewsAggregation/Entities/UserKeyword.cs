using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NewsAggregation.Configurations;

namespace NewsAggregation.Entities
{
    public class UserKeyword : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserKeywordId { get; set; }
        public string Keyword { get; set; }
        public bool IsEnabled { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }

    }
}
