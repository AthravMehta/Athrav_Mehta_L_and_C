using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using NewsAggregation.Configurations;

namespace NewsAggregation.Entities
{
    public class UserKeyword : BaseAuditEntity, BaseKeyEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User? User { get; set; }

        [ForeignKey(nameof(Category))]
        public Guid CategoryId { get; set; }
        public string Keyword { get; set; }
        public bool isEnabled { get; set; }
    }
}
