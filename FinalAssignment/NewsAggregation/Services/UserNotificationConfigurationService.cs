using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserNotificationConfigurationService : IUserNotificationConfigurationService
    {

        private readonly ICrudBaseRepository<User> _userRepository;
        private readonly ICrudBaseRepository<Category> _categoryRepository;
        private readonly ICrudBaseRepository<UserNotificationConfiguration> _crudBaseRepository;
        private readonly IUserNotificationConfigurationRepository _userNotificationConfigurationRepository;

        public UserNotificationConfigurationService(ICrudBaseRepository<User> userRepository, ICrudBaseRepository<Category> categoryRepository, 
            ICrudBaseRepository<UserNotificationConfiguration> crudBaseRepository, IUserNotificationConfigurationRepository userNotificationConfigurationRepository)
        {
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _crudBaseRepository = crudBaseRepository;
            _userNotificationConfigurationRepository = userNotificationConfigurationRepository;
        }

        public async Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto)
        {
            var entity = new UserNotificationConfiguration
            {
                UserId = dto.UserId,
                IsEnabled = dto.IsEnabled
            };

            await _crudBaseRepository.AddAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            dto.UserNotificationConfigurationId = entity.UserNotificationConfigurationId;
            return dto;
        }

        public async Task<UserNotificationConfigurationDto> UpdateAsync(int id, UserNotificationConfigurationDto dto)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity.UserId = dto.UserId;
            entity.IsEnabled = dto.IsEnabled;

            await _crudBaseRepository.UpdateAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            dto.UserNotificationConfigurationId = entity.UserNotificationConfigurationId;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await _crudBaseRepository.DeleteAsync(id);
                await _crudBaseRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllConfigurationAsync()
        {
            var entities = await _crudBaseRepository.GetAllAsync();

            return entities.Select(entity => new UserNotificationConfigurationDto
            {
                UserNotificationConfigurationId = entity.UserNotificationConfigurationId,
                UserId = entity.UserId,
                IsEnabled = entity.IsEnabled
            });
        }
        
        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllUserConfigurationAsync()
        {
            var entities = await _userNotificationConfigurationRepository.GetAllUserConfigurationAsync();

            return entities.Select(entity => new UserNotificationConfigurationDto
            {
                UserNotificationConfigurationId = entity.UserNotificationConfigurationId,
                UserId = entity.UserId,
                IsEnabled = entity.IsEnabled
            });
        }

        public async Task<bool> ExistsAsync(int userId, int categoryId)
        {
            return await _userNotificationConfigurationRepository.ExistsAsync(userId, categoryId);
        }

        public Task SaveChangesAsync()
        {
            return _crudBaseRepository.SaveChangesAsync();
        }



        /// <summary>
        /// Initializes UserNotificationConfiguration for all existing users and categories,
        /// creating missing configurations with IsEnabled = true.
        /// </summary>
        public async Task InitializeNotificationConfigurationsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            foreach (var user in users)
            {
                foreach (var category in categories)
                {
                    bool exists = await _userNotificationConfigurationRepository.ExistsAsync(user.UserId, category.CategoryId);
                    if (!exists)
                    {
                        var config = new UserNotificationConfiguration
                        {
                            UserId = user.UserId,
                            CategoryId = category.CategoryId,
                            IsEnabled = true
                        };
                        await _crudBaseRepository.AddAsync(config);
                    }
                }
            }

            await _crudBaseRepository.SaveChangesAsync();
        }
    }
}
