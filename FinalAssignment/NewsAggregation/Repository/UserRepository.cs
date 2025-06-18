using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly NewsAggregationDbContext _dbContext;

        public UserRepository(NewsAggregationDbContext dbContext)
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
