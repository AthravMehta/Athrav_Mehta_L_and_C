using AutoMapper;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
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
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CrudBaseService<User>> _logger;
        private readonly IUserNotificationConfigurationService _userNotificationConfigurationService;

        public UserService(
            IMapper mapper,
            IUserRepository userRepository,
            IJwtTokenService jwtTokenService,
            IEncryptionService encryptionService,
            ICrudBaseRepository<User> repository,
            ILogger<CrudBaseService<User>> logger,
            IUserNotificationConfigurationService userNotificationConfigurationService)
            : base(repository, logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _userNotificationConfigurationService = userNotificationConfigurationService ?? throw new ArgumentNullException(nameof(userNotificationConfigurationService));
        }

        public async Task<UserDataWithTokenDto> CreateUserAsync(UserCreateDto userDto)
        {
            ValidateUserCreateDto(userDto);

            var existingUser = await _userRepository.GetUserByName(userDto.Username);
            if (existingUser != null)
            {
                throw new ApiException(ErrorResponse.ErrorEnum.Duplicate, ErrorMessages.DuplicateEntry);
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

        public async Task<IEnumerable<UserReadDto>> GetAllUsersAsync(RoleEnum role = RoleEnum.User)
        {
            var users = await _userRepository.GetAllUsersAsync(role);
            if (users == null || users.Count == 0)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<IEnumerable<UserReadDto>>(users);
        }

        public async Task<User> GetUserByName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidModelState);

            var user = await _userRepository.GetUserByName(username);
            if (user == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return user;
        }

        public bool VerifyPassword(User user, string providedPassword)
        {
            if (user == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
            if (string.IsNullOrWhiteSpace(providedPassword))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidModelState);

            return _encryptionService.Verify(user.PasswordHash, providedPassword);
        }

        private string HashPassword(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidModelState);

            return _encryptionService.Encrypt(password);
        }

        private void ValidateUserCreateDto(UserCreateDto dto)
        {
            if (dto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            if (string.IsNullOrWhiteSpace(dto.Username) || dto.Username.Length < 8)
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.UsernameValidationFailed);

            if (string.IsNullOrWhiteSpace(dto.Password) || !IsValidPassword(dto.Password))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.PasswordValidationFailed);

            if (string.IsNullOrWhiteSpace(dto.Email) || !IsValidEmail(dto.Email))
                throw new ApiException(ErrorResponse.ErrorEnum.Validation, ErrorMessages.InvalidEmailFormat);
        }

        private bool IsValidPassword(string password)
        {
            // At least 8 chars, 1 uppercase, 1 lowercase, 1 special char
            var regex = new Regex(AppConstants.PasswordValidationRegex);
            return regex.IsMatch(password);
        }

        private bool IsValidEmail(string email)
        {
            // Simple email regex pattern (@)(.)
            var regex = new Regex(AppConstants.EmailValidationRegex);
            return regex.IsMatch(email);
        }
    }
}
