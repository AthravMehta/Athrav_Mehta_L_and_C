using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{

    // TODO: Modify CRUD BASE SERVICE and then edit this external server
    public class UserKeywordService : IUserKeywordService
    {
        private readonly ICrudBaseRepository<UserKeyword> _userKeywordRepo;
        public UserKeywordService(ICrudBaseRepository<UserKeyword> userKeywordRepo)
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
                IsEnabled = dto.IsEnabled
            };

            await _userKeywordRepo.AddAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            dto.UserKeywordId = entity.UserKeywordId;
            return dto;
        }

        public async Task<UserKeywordDto> UpdateAsync(int id, UserKeywordDto dto)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.UserId = dto.UserId;
            entity.CategoryId = dto.CategoryId;
            entity.Keyword = dto.Keyword;
            entity.IsEnabled = dto.IsEnabled;

            await _userKeywordRepo.UpdateAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            dto.UserKeywordId = entity.UserKeywordId;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity != null)
            {
                await _userKeywordRepo.DeleteAsync(id);
                await _userKeywordRepo.SaveChangesAsync();
            }
        }

        public async Task<UserKeywordDto> GetByIdAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            return new UserKeywordDto
            {
                UserKeywordId = entity.UserKeywordId,
                UserId = entity.UserId,
                CategoryId = entity.CategoryId,
                Keyword = entity.Keyword,
                IsEnabled = entity.IsEnabled
            };
        }

        public async Task<IEnumerable<UserKeywordDto>> GetAllAsync()
        {
            var entities = await _userKeywordRepo.GetAllAsync();
            return entities.Select(entity => new UserKeywordDto
            {
                UserKeywordId = entity.UserKeywordId,
                UserId = entity.UserId,
                CategoryId = entity.CategoryId,
                Keyword = entity.Keyword,
                IsEnabled = entity.IsEnabled
            });
        }
    }
}
