using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserByName(string username);
        Task<List<UserReadDto>> GetAllUsersAsync(RoleEnum role = RoleEnum.User);
    }
}
