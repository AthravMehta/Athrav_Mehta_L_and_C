using Microsoft.EntityFrameworkCore;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DbContext _dbContext;

        public UserRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<User> GetUserByName(string username)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
