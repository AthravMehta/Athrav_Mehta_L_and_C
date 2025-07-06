using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IAuthService
    {
        /// <summary>
        /// To Login User
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        Task<UserDataWithTokenDto> LoginAsync(LoginDto userDto);
    }
}
