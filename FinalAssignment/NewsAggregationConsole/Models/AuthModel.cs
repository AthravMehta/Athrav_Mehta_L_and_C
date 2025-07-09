namespace NewsAggregationConsole.Models
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserDataWithTokenDto
    {
        public UserReadDto User { get; set; }
        public string token { get; set; }
    }
}
