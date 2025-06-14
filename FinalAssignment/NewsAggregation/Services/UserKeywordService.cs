using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserKeywordService : IUserKeywordService
    {
        private readonly ICrudBaseRepository<UserKeyword, int> _userKeywordRepo;
        public UserKeywordService(ICrudBaseRepository<UserKeyword, int> userKeywordRepo)
        {
            _userKeywordRepo = userKeywordRepo;
        }

        // TODO: Configure Mapper here
        public async Task<UserKeywordDto> AddAsync(UserKeywordDto dto)
        {
            var entity = new UserKeyword
            {
                UserId = dto.UserId,
                CategoryId = dto.CategoryId,
                Keyword = dto.Keyword,
                isEnabled = dto.IsEnabled
            };

            await _userKeywordRepo.AddAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<UserKeywordDto> UpdateAsync(int id, UserKeywordDto dto)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.UserId = dto.UserId;
            entity.CategoryId = dto.CategoryId;
            entity.Keyword = dto.Keyword;
            entity.isEnabled = dto.IsEnabled;

            _userKeywordRepo.Update(entity);
            await _userKeywordRepo.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity != null)
            {
                _userKeywordRepo.Delete(entity);
                await _userKeywordRepo.SaveChangesAsync();
            }
        }

        public async Task<UserKeywordDto> GetByIdAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            return new UserKeywordDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CategoryId = entity.CategoryId,
                Keyword = entity.Keyword,
                IsEnabled = entity.isEnabled
            };
        }

        public async Task<IEnumerable<UserKeywordDto>> GetAllAsync()
        {
            var entities = await _userKeywordRepo.GetAllAsync();
            return entities.Select(entity => new UserKeywordDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                CategoryId = entity.CategoryId,
                Keyword = entity.Keyword,
                IsEnabled = entity.isEnabled
            });
        }
    }
}
