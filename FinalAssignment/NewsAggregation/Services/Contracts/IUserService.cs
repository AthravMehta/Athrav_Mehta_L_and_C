using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

public interface IUserService : ICrudBaseService<User, Guid>
{
    Task<UserReadDto> CreateUserAsync(UserCreateDto dto);
    Task<UserReadDto> UpdateUserAsync(Guid id, UserUpdateDto dto);
    Task<UserReadDto> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserReadDto>> GetAllUsersAsync();
    Task<User> GetUserByName(string username);
    bool VerifyPassword(User user, string providedPassword);
}
