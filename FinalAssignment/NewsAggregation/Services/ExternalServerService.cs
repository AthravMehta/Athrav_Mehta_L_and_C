using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class ExternalServerService : IExternalServerService
    {
        private readonly ICrudBaseRepository<ExternalServer> _crudBaseRepository;
        private readonly IExternalServerRepository _externalServerRepository;
        private readonly EncryptionService _encryptionService;
        private readonly IMapper _mapper;

        public ExternalServerService(
            ICrudBaseRepository<ExternalServer> crudBaseRepository,
            IExternalServerRepository externalServerRepository,
            EncryptionService encryptionService,
            IMapper mapper)
        {
            _crudBaseRepository = crudBaseRepository;
            _externalServerRepository = externalServerRepository;
            _encryptionService = encryptionService;
            _mapper = mapper;
        }

        public async Task<ExternalServerDto> AddAsync(ExternalServerDto dto)
        {
            var entity = _mapper.Map<ExternalServer>(dto);
            entity.ApiKeyHash = _encryptionService.Encrypt(dto.ApiKeyHash);

            await _crudBaseRepository.AddAsync(entity);
            await _crudBaseRepository.SaveChangesAsync();

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<ExternalServerDto> UpdateAsync(int id, ExternalServerDto dto)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            entity = _mapper.Map(dto, entity);
            entity.ApiKeyHash = _encryptionService.Encrypt(dto.ApiKeyHash);

            await _crudBaseRepository.SaveChangesAsync();

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<ExternalServerDto> GetByIdAsync(int id)
        {
            var entity = await _crudBaseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return _mapper.Map<ExternalServerDto>(entity);
        }

        public async Task<IEnumerable<ExternalServerDto>> GetAllAsync(bool? isActiveFilter = null)
        {
            var entities = await _externalServerRepository.GetAllAsync(isActiveFilter);
            return _mapper.Map<IEnumerable<ExternalServerDto>>(entities);
        }
    }
}
