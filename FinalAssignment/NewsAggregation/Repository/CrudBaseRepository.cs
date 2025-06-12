using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class CrudBaseRepository<TEntity, TKey> : ICrudBaseRepository<TEntity, TKey> where TEntity : class, BaseKeyEntity<TKey>
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _entitySet;

        public CrudBaseRepository(DbContext context)
        {
            _context = context;
            _entitySet = context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity) =>
            await _entitySet.AddAsync(entity);

        public void Delete(TEntity entity) =>
            _entitySet.Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _entitySet.ToListAsync();

        public async Task<TEntity> GetByIdAsync(TKey id) =>
            await _entitySet.FindAsync(id);
        

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();
        

        public void Update(TEntity entity) =>
            _entitySet.Update(entity);
    }
}
