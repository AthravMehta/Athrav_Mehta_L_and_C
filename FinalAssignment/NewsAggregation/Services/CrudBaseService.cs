using NewsAggregation.Configurations;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class CrudBaseService<TEntity, TKey> : ICrudBaseService<TEntity, TKey> where TEntity : BaseKeyEntity<TKey>
    {
        private readonly ICrudBaseRepository<TEntity, TKey> _repository;

        public CrudBaseService(ICrudBaseRepository<TEntity, TKey> repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<TEntity> GetByIdAsync(TKey id) =>
            await _repository.GetByIdAsync(id);

        public async Task AddAsync(TEntity entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Delete(entity);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
