using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Enums;
using Polly;

namespace NewsAggregation.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly NewsAggregationDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepository(NewsAggregationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(DbContext));
            _mapper = mapper;   
        }
        public async Task<User> GetUserByName(string username)
        {
            return await _dbContext.Set<User>()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<List<UserReadDto>> GetAllUsersAsync()
        {
            var users = await _dbContext.Users
                .Include(u => u.UserSavedArticles)
                .Include(u => u.UserNotifications)
                .Include(u => u.UserNotificationConfigurations)
                .Include(u => u.UserKeywords)
                .Where(user => user.RoleId != RoleEnum.Admin)
                .ToListAsync();

            return _mapper.Map<List<UserReadDto>>(users);
        }
    }
}
