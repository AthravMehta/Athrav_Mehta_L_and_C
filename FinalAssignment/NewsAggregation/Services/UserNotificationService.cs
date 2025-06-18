using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly ICrudBaseRepository<UserNotification> _repo;

        public UserNotificationService(ICrudBaseRepository<UserNotification> repo)
        {
            _repo = repo;
        }

        public async Task<UserNotificationDto> AddAsync(UserNotificationDto dto)
        {
            var entity = new UserNotification
            {
                UserId = dto.UserId,
                ArticleId = dto.ArticleId,
                SentDateTime = dto.SentDateTime,
                IsRead = dto.IsRead
            };

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<UserNotificationDto> UpdateAsync(int id, UserNotificationDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.UserId = dto.UserId;
            entity.ArticleId = dto.ArticleId;
            entity.SentDateTime = dto.SentDateTime;
            entity.IsRead = dto.IsRead;

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

        public async Task<IEnumerable<UserNotificationDto>> GetAllAsync(int? userId = null)
        {
            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new UserNotificationDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ArticleId = entity.ArticleId,
                SentDateTime = entity.SentDateTime,
                IsRead = entity.IsRead
            });
        }
    }
}
