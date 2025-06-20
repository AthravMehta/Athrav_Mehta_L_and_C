using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserKeywordService : IUserKeywordService
    {
        private readonly IMapper _mapper;
        private readonly ICrudBaseRepository<UserKeyword> _userKeywordRepo;
        public UserKeywordService(IMapper mapper, ICrudBaseRepository<UserKeyword> userKeywordRepo)
        {
            _userKeywordRepo = userKeywordRepo;
            _mapper = mapper;
        }

        public async Task<UserKeywordDto> AddAsync(UserKeywordDto userKeywordDto)
        {
            var entity = _mapper.Map<UserKeyword>(userKeywordDto);

            await _userKeywordRepo.AddAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            userKeywordDto.UserKeywordId = entity.UserKeywordId;
            return userKeywordDto;
        }

        public async Task<UserKeywordDto> UpdateAsync(int id, UserKeywordDto userKeywordDto)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null) return null;

            entity = _mapper.Map<UserKeyword>(userKeywordDto);

            await _userKeywordRepo.UpdateAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            userKeywordDto.UserKeywordId = entity.UserKeywordId;
            return userKeywordDto;
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

            return _mapper.Map<UserKeywordDto>(entity);
        }

        public async Task<IEnumerable<UserKeywordDto>> GetAllAsync()
        {
            var entities = await _userKeywordRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<UserKeywordDto>>(entities);
        }
    }
}
