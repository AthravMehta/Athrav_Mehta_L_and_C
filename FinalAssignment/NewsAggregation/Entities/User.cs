using System.ComponentModel.DataAnnotations;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;

namespace NewsAggregation.Entities
{
    public class User : BaseKeyEntity<Guid>
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)] // Suitable for hashed passwords (e.g., bcrypt)
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public DateTime? LastLoginDateTime { get; set; }

        [Required]
        public RoleEnum RoleId { get; set; } = RoleEnum.User;

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
