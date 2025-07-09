using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;

namespace NewsAggregation.Services.Contracts
{
    public interface IUserService
    {
        Task<UserDataWithTokenDto> CreateUserAsync(UserCreateDto dto);
        Task<IEnumerable<UserReadDto>> GetAllUsersAsync(RoleEnum role = RoleEnum.User);
        Task<User> GetUserByName(string username);
        bool VerifyPassword(User user, string providedPassword);
    }
}