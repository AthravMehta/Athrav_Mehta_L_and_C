using NewsAggregation.Configurations;

namespace NewsAggregation.Services.Contracts
{
    public interface ICrudBaseService<TEntity, TKey> where TEntity : BaseKeyEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TKey id);
    }
}
