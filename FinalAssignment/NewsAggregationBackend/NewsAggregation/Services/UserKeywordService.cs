using AutoMapper;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class UserKeywordService : IUserKeywordService
    {
        private readonly IMapper _mapper;
        private readonly ICrudBaseRepository<UserKeyword> _userKeywordRepo;

        public UserKeywordService(IMapper mapper, ICrudBaseRepository<UserKeyword> userKeywordRepo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userKeywordRepo = userKeywordRepo ?? throw new ArgumentNullException(nameof(userKeywordRepo));
        }

        public async Task<UserKeywordDto> AddAsync(UserKeywordDto userKeywordDto)
        {
            if (userKeywordDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = _mapper.Map<UserKeyword>(userKeywordDto);

            await _userKeywordRepo.AddAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            userKeywordDto.UserKeywordId = entity.UserKeywordId;
            return userKeywordDto;
        }

        public async Task<UserKeywordDto> UpdateAsync(int id, UserKeywordDto userKeywordDto)
        {
            if (userKeywordDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            // Map updated fields from DTO to entity
            _mapper.Map(userKeywordDto, entity);

            await _userKeywordRepo.UpdateAsync(entity);
            await _userKeywordRepo.SaveChangesAsync();

            userKeywordDto.UserKeywordId = entity.UserKeywordId;
            return userKeywordDto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            await _userKeywordRepo.DeleteAsync(id);
            await _userKeywordRepo.SaveChangesAsync();
        }

        public async Task<UserKeywordDto> GetByIdAsync(int id)
        {
            var entity = await _userKeywordRepo.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<UserKeywordDto>(entity);
        }

        public async Task<IEnumerable<UserKeywordDto>> GetAllAsync()
        {
            var entities = await _userKeywordRepo.GetAllAsync();
            if (entities == null || !entities.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<IEnumerable<UserKeywordDto>>(entities);
        }
    }
}
