using NewsAggregation.Entities;
using NewsAggregation.Enums;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserByName(string username);
        Task<List<User>> GetAllUsersAsync(RoleEnum role = RoleEnum.User);
    }
}
