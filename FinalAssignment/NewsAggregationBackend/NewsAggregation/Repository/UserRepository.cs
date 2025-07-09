using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly NewsAggregationDbContext _dbContext;

        public UserRepository(NewsAggregationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<User> GetUserByName(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException(nameof(username));

            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<User>> GetAllUsersAsync(RoleEnum role = RoleEnum.User)
        {
            var users = await _dbContext.Users
                .Include(u => u.UserSavedArticles)
                .Include(u => u.UserNotifications)
                .Include(u => u.UserNotificationConfigurations)
                .Include(u => u.UserKeywords)
                .Where(user => user.RoleId == role)
                .ToListAsync();

            return users ?? new List<User>();
        }
    }
}
