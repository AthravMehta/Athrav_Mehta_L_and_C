using AutoMapper;
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
        private readonly IMapper _mapper;


        public AuthService(IUserService userService, IJwtTokenService jwtTokenService, IMapper mapper)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<UserDataWithTokenDto> RegisterAsync(UserCreateDto userDto)
        {
            var existingUser = await _userService.GetUserByName(userDto.Username);
            if (existingUser != null)
            {
                throw new ApiException("User already exist!");
            }
            var newUser = await _userService.CreateUserAsync(userDto);
            var roles = new List<string> { newUser.RoleId.ToString() };
            var userDataWithToken = new UserDataWithTokenDto
            {
                User = newUser,
                token = _jwtTokenService.GenerateToken(newUser.Id.ToString(), newUser.Username, roles)
            };
            return userDataWithToken;
        }

        public async Task<UserDataWithTokenDto> LoginAsync(LoginDto userDto)
        {
            User user = await _userService.GetUserByName(userDto.Username);
            if (user == null)
                throw new ApiException("Invalid Username. User Don't Exist!");

            if (!_userService.VerifyPassword(user, userDto.Password))
                throw new ApiException("Invalid username or password.");

            var roles = new List<string> { user.RoleId.ToString() };
            var userDataWithToken = new UserDataWithTokenDto
            {
                User = _mapper.Map<UserReadDto>(user),
                token = _jwtTokenService.GenerateToken(user.Id.ToString(), user.Username, roles)
            };
            return userDataWithToken;
        }
    }
}
