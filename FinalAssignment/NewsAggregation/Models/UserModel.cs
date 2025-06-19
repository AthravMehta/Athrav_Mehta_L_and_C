using NewsAggregation.Entities;
using NewsAggregation.Enums;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregation.Models
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        public RoleEnum RoleId { get; set; } = RoleEnum.User;
    }

    // DTO for updating a user (input)
    public class UserUpdateDto
    {
        [MaxLength(50)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        [MaxLength(255)]
        public string Email { get; set; }

        public RoleEnum? RoleId { get; set; }
    }

    public class UserReadDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleEnum RoleId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
        public ICollection<UserSavedArticleDto> UserSavedArticles { get; set; }
        public ICollection<UserNotificationDto> UserNotifications { get; set; }
        public ICollection<UserNotificationConfigurationDto> UserNotificationConfigurations { get; set; }
        public ICollection<UserKeywordDto> UserKeywords { get; set; }
    }
}
