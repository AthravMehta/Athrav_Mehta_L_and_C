using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{

    public class CrudBaseRepository<TEntity> : ICrudBaseRepository<TEntity> where TEntity : class
    {
        protected readonly NewsAggregationDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public CrudBaseRepository(NewsAggregationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public virtual async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
