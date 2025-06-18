using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    // TODO: Modify CRUD BASE SERVICE and then edit this external server
    public class ExternalServerService : IExternalServerService
    {
        private readonly ICrudBaseRepository<ExternalServer> _repo;

        public ExternalServerService(ICrudBaseRepository<ExternalServer> repo)
        {
            _repo = repo;
        }

        public async Task<ExternalServerDto> AddAsync(ExternalServerDto dto)
        {
            var entity = new ExternalServer
            {
                ServerName = dto.ServerName,
                BaseUrl = dto.BaseUrl,
                ApiKeyHash = dto.ApiKeyHash,
                IsActive = dto.IsActive
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.ExternalServerId;
            return dto;
        }

        public async Task<ExternalServerDto> UpdateAsync(int id, ExternalServerDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ServerName = dto.ServerName;
            entity.BaseUrl = dto.BaseUrl;
            entity.ApiKeyHash = dto.ApiKeyHash;
            entity.IsActive = dto.IsActive;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.ExternalServerId;
            return dto;
        }

        public async Task<ExternalServerDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new ExternalServerDto
            {
                Id = entity.ExternalServerId,
                ServerName = entity.ServerName,
                BaseUrl = entity.BaseUrl,
                ApiKeyHash = entity.ApiKeyHash,
                IsActive = entity.IsActive
            };
        }

        public async Task<IEnumerable<ExternalServerDto>> GetAllAsync()
        {

            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new ExternalServerDto
            {
                Id = entity.ExternalServerId,
                ServerName = entity.ServerName,
                BaseUrl = entity.BaseUrl,
                ApiKeyHash = entity.ApiKeyHash,
                IsActive = entity.IsActive
            });
        }
    }
}
