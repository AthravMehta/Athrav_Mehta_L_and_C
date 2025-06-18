using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserNotificationConfigurationService : IUserNotificationConfigurationService
    {
        private readonly ICrudBaseRepository<UserNotificationConfiguration> _repo;

        public UserNotificationConfigurationService(ICrudBaseRepository<UserNotificationConfiguration> repo)
        {
            _repo = repo;
        }

        public async Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto)
        {
            var entity = new UserNotificationConfiguration
            {
                UserId = dto.UserId,
                IsEnabled = dto.IsEnabled
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<UserNotificationConfigurationDto> UpdateAsync(int id, UserNotificationConfigurationDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.UserId = dto.UserId;
            entity.IsEnabled = dto.IsEnabled;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                await _repo.DeleteAsync(id);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllAsync()
        {
            // TODO: Get User Specific Configurations Only
            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new UserNotificationConfigurationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                IsEnabled = entity.IsEnabled
            });
        }
    }
}
