using NewsAggregationConsole.Enums;

namespace NewsAggregationConsole.Models
{
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public RoleEnum RoleId { get; set; }
    }
    public class UserUpdateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public RoleEnum? RoleId { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public RoleEnum RoleId { get; set; }
        public DateTime? LastLoginDateTime { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime LastUpdatedDateTime { get; set; }
    }
}
