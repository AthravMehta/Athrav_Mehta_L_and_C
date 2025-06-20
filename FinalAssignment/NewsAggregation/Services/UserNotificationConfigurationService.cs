using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserNotificationConfigurationService : IUserNotificationConfigurationService
    {
        private readonly IMapper _mapper;
        private readonly ICrudBaseRepository<User> _userRepository;
        private readonly ICrudBaseRepository<Category> _categoryRepository;
        private readonly ICrudBaseRepository<UserNotificationConfiguration> _crudBaseRepository;
        private readonly IUserNotificationConfigurationRepository _userNotificationConfigurationRepository;

        public UserNotificationConfigurationService(IMapper mapper, ICrudBaseRepository<User> userRepository, ICrudBaseRepository<Category> categoryRepository,
            ICrudBaseRepository<UserNotificationConfiguration> crudBaseRepository, IUserNotificationConfigurationRepository userNotificationConfigurationRepository)
        {
            _mapper = mapper;
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

            return _mapper.Map<IEnumerable<UserNotificationConfigurationDto>>(entities);
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllUserConfigurationAsync()
        {
            var entities = await _userNotificationConfigurationRepository.GetAllUserConfigurationAsync();

            return _mapper.Map<IEnumerable<UserNotificationConfigurationDto>>(entities);
        }

        public async Task<bool> ExistsAsync(int userId, int categoryId)
        {
            return await _userNotificationConfigurationRepository.ExistsAsync(userId, categoryId);
        }

        public Task SaveChangesAsync()
        {
            return _crudBaseRepository.SaveChangesAsync();
        }

        public async Task CreateNotificationConfigForAllUsersAsync(int? categoryId = null, User? newUser = null)
        {
            if (newUser != null)
            {
                var categories = await _categoryRepository.GetAllAsync();
                foreach (var category in categories)
                {
                    bool exists = await _userNotificationConfigurationRepository.ExistsAsync(newUser.UserId, category.CategoryId);
                    if (!exists)
                    {
                        var config = new UserNotificationConfiguration
                        {
                            UserId = newUser.UserId,
                            CategoryId = category.CategoryId,
                            IsEnabled = true
                        };
                        await _crudBaseRepository.AddAsync(config);
                    }
                }
            }
            else if (categoryId != null)
            {
                var users = await _userRepository.GetAllAsync();

                foreach (var user in users)
                {
                    bool exists = await this.ExistsAsync(user.UserId, categoryId!.Value);
                    if (!exists)
                    {
                        var config = new UserNotificationConfigurationDto
                        {
                            UserId = user.UserId,
                            CategoryId = categoryId!.Value,
                            IsEnabled = true
                        };
                        await this.AddAsync(config);
                    }
                }
            }
            await this.SaveChangesAsync();
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
