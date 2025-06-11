using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;


        public AuthService(IUserService userService, IJwtTokenService jwtTokenService)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<string> RegisterAsync(UserCreateDto userDto)
        {
            var existingUser = await _userService.GetUserByName(userDto.Username);
            if (existingUser != null)
            {
                throw new ApiException("User already exist!");
            }
            var newUser = await _userService.CreateUserAsync(userDto);
            var roles = new List<string> { newUser.RoleId.ToString() };
            return _jwtTokenService.GenerateToken(newUser.Id.ToString(), newUser.Username, roles);
        }

        public async Task<string> LoginAsync(LoginDto userDto)
        {
            User user = await _userService.GetUserByName(userDto.Username);
            if (user == null)
                throw new ApiException("Invalid Username. User Don't Exist!");

            if (!_userService.VerifyPassword(user, userDto.Password))
                throw new ApiException("Invalid username or password.");

            var roles = new List<string> { user.RoleId.ToString() };
            return _jwtTokenService.GenerateToken(user.Id.ToString(), user.Username, roles);
        }
    }
}
