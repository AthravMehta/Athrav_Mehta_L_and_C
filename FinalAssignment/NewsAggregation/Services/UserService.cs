using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using System.Text.RegularExpressions;

namespace NewsAggregation.Services
{
    public class UserService : CrudBaseService<User>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CrudBaseService<User>> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public UserService(ICrudBaseRepository<User> repository, IMapper mapper, IUserRepository userRepository, ILogger<CrudBaseService<User>> logger, IJwtTokenService jwtTokenService) : base(repository, logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<UserDataWithTokenDto> CreateUserAsync(UserCreateDto userDto)
        {
            ValidateUserCreateDto(userDto);

            var existingUser = await _userRepository.GetUserByName(userDto.Username);
            if (existingUser != null)
            {
                throw new ApiException("User already exists!");
            }

            var user = _mapper.Map<User>(userDto);
            user.PasswordHash = HashPassword(user, userDto.Password);
            user.CreatedDateTime = DateTime.UtcNow;
            user.LastUpdatedDateTime = DateTime.UtcNow;

            await AddAsync(user);

            var userReadDto = _mapper.Map<UserReadDto>(user);

            var roles = new List<string> { userReadDto.RoleId.ToString() };
            var token = _jwtTokenService.GenerateToken(user.UserId.ToString(), user.Username, user.Email, roles);

            return new UserDataWithTokenDto
            {
                User = userReadDto,
                token = token
            };
        }
        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync()
        {
            var users = await GetAllAsync();
            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<User> GetUserByName(string username)
        {
            var user = await _userRepository.GetUserByName(username);
            return user;
        }

        public bool VerifyPassword(User user, string providedPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, providedPassword);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }

        private string HashPassword(User user, string password)
        {
            return _passwordHasher.HashPassword(user, password);
        }

        // TODO: Proper Exception code can be returned.
        private void ValidateUserCreateDto(UserCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 8)
                throw new ApiException("Username must be at least 8 characters long.");

            if (string.IsNullOrWhiteSpace(dto.Password) || !IsValidPassword(dto.Password))
                throw new ApiException("Password must be at least 8 characters long and contain uppercase, lowercase, and special character.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                throw new ApiException("Email format is invalid.");
        }

        private bool IsValidPassword(string password)
        {
            // At least 8 chars, 1 uppercase, 1 lowercase, 1 special char
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$");
            return regex.IsMatch(password);
        }

        private bool IsValidEmail(string email)
        {
            // Simple email regex pattern (@)(.)
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }

    }
}
