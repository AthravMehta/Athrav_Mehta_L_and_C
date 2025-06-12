using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;

namespace NewsAggregation.Entities
{
    public class User : BaseKeyEntity<Guid>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public DateTime? LastLoginDateTime { get; set; }

        [Required]
        public RoleEnum RoleId { get; set; } = RoleEnum.User;

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedDateTime { get; set; } = DateTime.UtcNow;

        public ICollection<UserArticleAction>? UserArticleActions { get; set; }
        public ICollection<UserNotification>? Notifications { get; set; }
        public ICollection<UserNotificationConfiguration>? NotificationConfigurations { get; set; }
        public ICollection<UserKeyword> UserKeywords { get; set; }
    }
}
