using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IAuthService
    {
        Task<UserDataWithTokenDto> LoginAsync(LoginDto userDto);
    }
}
