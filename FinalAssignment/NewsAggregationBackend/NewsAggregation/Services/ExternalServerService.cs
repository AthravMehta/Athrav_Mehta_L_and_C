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
    public class ExternalServerService : IExternalServerService
    {
        private readonly ICrudBaseRepository<ExternalServer> _crudBaseRepository;
        private readonly IExternalServerRepository _externalServerRepository;
        private readonly IEncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public ExternalServerService(
            ICrudBaseRepository<ExternalServer> crudBaseRepository,
            IExternalServerRepository externalServerRepository,
            IEncryptionService encryptionService,
            IMapper mapper)
        {
            _crudBaseRepository = crudBaseRepository ?? throw new ArgumentNullException(nameof(crudBaseRepository));
            _externalServerRepository = externalServerRepository ?? throw new ArgumentNullException(nameof(externalServerRepository));
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ExternalServerDto> AddAsync(ExternalServerDto dto)
        {
            if (dto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = _mapper.Map<ExternalServer>(dto);
            entity.ApiKeyHash = _encryptionService.Encrypt(dto.ApiKeyHash);

            await _crudBaseRepository.AddAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<ExternalServerDto> UpdateAsync(int id, ExternalServerDto dto)
        {
            if (dto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            entity = _mapper.Map(dto, entity);
            entity.ApiKeyHash = _encryptionService.Encrypt(dto.ApiKeyHash);

            await _crudBaseRepository.SaveChangesAsync();

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<ExternalServerDto> GetByIdAsync(int id)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<IEnumerable<ExternalServerDto>> GetAllAsync(bool? isActiveFilter = null)
        {
            var entities = await _externalServerRepository.GetAllAsync(isActiveFilter);
            if (entities == null || !entities.Any())
                throw new ApiException(ErrorResponse.ErrorEnum.NotFound, ErrorMessages.ResourceNotFound);

            return _mapper.Map<IEnumerable<ExternalServerDto>>(entities);
        }
    }
}
