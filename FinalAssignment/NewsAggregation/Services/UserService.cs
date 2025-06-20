using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using System.Text.RegularExpressions;

namespace NewsAggregation.Services
{
    public class UserService : CrudBaseService<User>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly EncryptionService _encryptionService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CrudBaseService<User>> _logger;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserNotificationConfigurationService _userNotificationConfigurationService;

        public UserService(ICrudBaseRepository<User> repository, IMapper mapper, IUserRepository userRepository, ILogger<CrudBaseService<User>> logger, 
            IJwtTokenService jwtTokenService, EncryptionService encryptionService, IUserNotificationConfigurationService userNotificationConfigurationService)
            : base(repository, logger)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
            _encryptionService = encryptionService;
            _userNotificationConfigurationService = userNotificationConfigurationService;
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
            await _userNotificationConfigurationService.CreateNotificationConfigForAllUsersAsync(null, user);
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
            var users = await _userRepository.GetAllUsersAsync();
            return users;
        }

        public async Task<User> GetUserByName(string username)
        {
            var user = await _userRepository.GetUserByName(username);
            return user;
        }

        public bool VerifyPassword(User user, string providedPassword)
        {
            var result = _encryptionService.Verify(user.PasswordHash, providedPassword);
            return result;
        }

        private string HashPassword(User user, string password)
        {
            return _encryptionService.Encrypt(password);
        }

        private void ValidateUserCreateDto(UserCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 8)
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, "Username must be at least 8 characters long.");

            if (string.IsNullOrWhiteSpace(dto.Password) || !IsValidPassword(dto.Password))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, "Password must be at least 8 characters long and contain uppercase, lowercase, and special character.");

            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, "Email format is invalid.");
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
