using NewsAggregation.Entities;
using NewsAggregation.Models;

public interface IUserService
{
    Task<UserDataWithTokenDto> CreateUserAsync(UserCreateDto dto);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
    Task<User> GetUserByName(string username);
    bool VerifyPassword(User user, string providedPassword);
}
