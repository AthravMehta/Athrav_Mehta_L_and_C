using AutoMapper;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMapper _mapper;

        public AuthService(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IMapper mapper)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _mapper = mapper;
        }

        public async Task<UserDataWithTokenDto> LoginAsync(LoginDto loginDto)
        {
            if (loginDto == null)
            {
                throw new ApiException(
                    ErrorResponse.ErrorEnum.NullObject,
                    ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));
            }

            var user = await _userService.GetUserByName(loginDto.Username);

            if (user == null)
            {
                throw new ApiException(
                    ErrorResponse.ErrorEnum.NotFound,
                    ErrorMessages.ResourceNotFound);
            }

            if (!_userService.VerifyPassword(user, loginDto.Password))
            {
                throw new ApiException(
                    ErrorResponse.ErrorEnum.Validation,
                    ErrorMessages.InvalidPassword);
            }

            var roles = new List<string> { user.RoleId.ToString() };

            return new UserDataWithTokenDto
            {
                User = _mapper.Map<UserReadDto>(user),
                token = _jwtTokenService.GenerateToken(user.UserId.ToString(), user.Username, user.Email, roles)
            };
        }
    }
}
