using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;

namespace NewsAggregation.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
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

        public ICollection<UserSavedArticle> UserSavedArticles{ get; set; } = new List<UserSavedArticle>();
        public ICollection<UserNotification> UserNotifications { get; set; } = new List<UserNotification>();
        public ICollection<UserNotificationConfiguration> UserNotificationConfigurations { get; set; } = new List<UserNotificationConfiguration>();
        public ICollection<UserKeyword> UserKeywords { get; set; } = new List<UserKeyword>();
    }
}
