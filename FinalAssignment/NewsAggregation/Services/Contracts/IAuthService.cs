using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserCreateDto userDto);
        Task<string> LoginAsync(LoginDto userDto);
    }
}
