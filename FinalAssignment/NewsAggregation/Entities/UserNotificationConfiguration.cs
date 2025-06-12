using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Entities
{
    public class UserNotificationConfiguration : BaseAuditEntity, BaseKeyEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid CategoryId { get; set; }
        public bool isEnabled { get; set; }
    }
}
