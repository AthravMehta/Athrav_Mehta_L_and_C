using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAggregation.Services
{
    public class ExternalServerService : IExternalServerService
    {
        private readonly ICrudBaseRepository<ExternalServer, Guid> _repo;

        public ExternalServerService(ICrudBaseRepository<ExternalServer, Guid> repo)
        {
            _repo = repo;
        }

        public async Task<ExternalServerDto> AddAsync(ExternalServerDto dto)
        {
            var entity = new ExternalServer
            {
                Id = Guid.NewGuid(),
                ServerName = dto.ServerName,
                BaseUrl = dto.BaseUrl,
                ApiKeyHash = dto.ApiKeyHash,
                isActive = dto.IsActive
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<ExternalServerDto> UpdateAsync(Guid id, ExternalServerDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ServerName = dto.ServerName;
            entity.BaseUrl = dto.BaseUrl;
            entity.ApiKeyHash = dto.ApiKeyHash;
            entity.isActive = dto.IsActive;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                _repo.Delete(entity);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<ExternalServerDto> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new ExternalServerDto
            {
                Id = entity.Id,
                ServerName = entity.ServerName,
                BaseUrl = entity.BaseUrl,
                ApiKeyHash = entity.ApiKeyHash,
                IsActive = entity.isActive
            };
        }

        public async Task<IEnumerable<ExternalServerDto>> GetAllAsync()
        {

            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new ExternalServerDto
            {
                Id = entity.Id,
                ServerName = entity.ServerName,
                BaseUrl = entity.BaseUrl,
                ApiKeyHash = entity.ApiKeyHash,
                IsActive = entity.isActive
            });
        }
    }
}
