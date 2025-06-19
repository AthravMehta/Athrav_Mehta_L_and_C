using NewsAggregation.Configurations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregation.Entities
{
    public class UserNotificationConfiguration : BaseAuditEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserNotificationConfigurationId { get; set; }
        public bool IsEnabled { get; set; }

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public ICollection<UserNotificationConfiguration> UserNotificationConfigurations { get; set; } = new List<UserNotificationConfiguration>();
    }
}
