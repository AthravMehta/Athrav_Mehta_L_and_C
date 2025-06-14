using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IAuthService
    {
        Task<UserDataWithTokenDto> RegisterAsync(UserCreateDto userDto);
        Task<UserDataWithTokenDto> LoginAsync(LoginDto userDto);
    }
}
