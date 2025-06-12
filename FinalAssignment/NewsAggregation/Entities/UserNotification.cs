using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Entities
{
    public class UserNotification : BaseAuditEntity, BaseKeyEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public int ArticleId { get; set; }
        public DateTime SentDateTime { get; set; }
        public bool IsRead { get; set; } = false;
    }
}
