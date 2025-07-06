using AutoMapper;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAggregation.Services
{
    public class UserNotificationConfigurationService : IUserNotificationConfigurationService
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly ICrudBaseRepository<Category> _categoryRepository;
        private readonly ICrudBaseRepository<UserNotificationConfiguration> _crudBaseRepository;
        private readonly IUserNotificationConfigurationRepository _userNotificationConfigurationRepository;

        public UserNotificationConfigurationService(
            IMapper mapper,
            IUserRepository userRepository,
            ICrudBaseRepository<Category> categoryRepository,
            ICrudBaseRepository<UserNotificationConfiguration> crudBaseRepository,
            IUserNotificationConfigurationRepository userNotificationConfigurationRepository)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _crudBaseRepository = crudBaseRepository ?? throw new ArgumentNullException(nameof(crudBaseRepository));
            _userNotificationConfigurationRepository = userNotificationConfigurationRepository ?? throw new ArgumentNullException(nameof(userNotificationConfigurationRepository));
        }

        public async Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto)
        {
            if (dto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = new UserNotificationConfiguration
            {
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                IsEnabled = dto.IsEnabled
            };

            await _crudBaseRepository.AddAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            dto.UserNotificationConfigurationId = entity.UserNotificationConfigurationId;
            return dto;
        }

        public async Task<UserNotificationConfigurationDto> UpdateAsync(int id, UserNotificationConfigurationDto dto)
        {
            if (dto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

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
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            await _crudBaseRepository.DeleteAsync(id);
            await _crudBaseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllConfigurationAsync()
        {
            var entities = await _crudBaseRepository.GetAllAsync();
            if (entities == null || !entities.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<IEnumerable<UserNotificationConfigurationDto>>(entities);
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllUserConfigurationAsync()
        {
            var entities = await _userNotificationConfigurationRepository.GetAllUserConfigurationAsync();
            if (entities == null || !entities.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

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
                if (categories == null || !categories.Any())
                    throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

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
                var users = await _userRepository.GetAllUsersAsync();
                if (users == null || !users.Any())
                    throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

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

        public async Task InitializeNotificationConfigurationsAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            var categories = await _categoryRepository.GetAllAsync();

            if (users == null || !users.Any() || categories == null || !categories.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

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
