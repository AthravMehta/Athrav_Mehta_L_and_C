using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NewsAggregation.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UserId { get; set; }

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
        public int RoleId { get; set; } = 1;

        public DateTime CreatedDateTime { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdatedDateTime { get; set; } = DateTime.UtcNow;
    }
}
