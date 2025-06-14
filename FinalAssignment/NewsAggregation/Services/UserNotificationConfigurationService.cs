using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserNotificationConfigurationService : IUserNotificationConfigurationService
    {
        private readonly ICrudBaseRepository<UserNotificationConfiguration, int> _repo;

        public UserNotificationConfigurationService(ICrudBaseRepository<UserNotificationConfiguration, int> repo)
        {
            _repo = repo;
        }

        public async Task<UserNotificationConfigurationDto> AddAsync(UserNotificationConfigurationDto dto)
        {
            var entity = new UserNotificationConfiguration
            {
                UserId = dto.UserId,
                isEnabled = dto.IsEnabled
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
            entity.isEnabled = dto.IsEnabled;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                _repo.Delete(entity);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<UserNotificationConfigurationDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new UserNotificationConfigurationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                IsEnabled = entity.isEnabled
            };
        }

        public async Task<IEnumerable<UserNotificationConfigurationDto>> GetAllAsync(Guid? userId = null)
        {
            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new UserNotificationConfigurationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                IsEnabled = entity.isEnabled
            });
        }
    }
}
