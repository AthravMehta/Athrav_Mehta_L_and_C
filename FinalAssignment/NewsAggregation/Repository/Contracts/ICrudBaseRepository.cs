using NewsAggregation.Configurations;

namespace NewsAggregation.Repository.Contracts
{
    public interface ICrudBaseRepository<TEntity, TKey> where TEntity : BaseKeyEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        void Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(TKey id);
        Task SaveChangesAsync();
        void Update(TEntity entity);
    }
}
