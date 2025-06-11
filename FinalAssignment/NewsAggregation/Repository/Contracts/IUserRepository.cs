using NewsAggregation.Entities;

namespace NewsAggregation.Repository.Contracts
{
    public interface IUserRepository
    {
        Task<User> GetUserByName(string username);
    }
}
