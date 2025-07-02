using NewsAggregation.Exceptions;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class CrudBaseService<TEntity> : ICrudBaseService<TEntity> where TEntity : class
    {
        private readonly ICrudBaseRepository<TEntity> _repository;
        private readonly ILogger<CrudBaseService<TEntity>> _logger;

        public CrudBaseService(
            ICrudBaseRepository<TEntity> repository,
            ILogger<CrudBaseService<TEntity>> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        protected virtual string EntityName => typeof(TEntity).Name;

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving all {EntityName} entities");
                throw new ApiException($"Failed to get all {EntityName} entities.", ex, _logger);
            }
        }

        public virtual async Task<TEntity> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving {EntityName} with ID {id}");
                throw new ApiException($"Failed to get {EntityName} with ID {id}.", ex, _logger);
            }
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            try
            {
                await _repository.AddAsync(entity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding new {EntityName}");
                throw new ApiException($"Failed to add new {EntityName}.", ex, _logger);
            }
        }

        public virtual async Task AddRangeAsync(List<TEntity> entities)
        {
            try
            {
                await _repository.AddRangeAsync(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding range of {EntityName} entities");
                throw new ApiException($"Failed to add range of {EntityName} entities.", ex, _logger);
            }
        }


        public virtual async Task UpdateAsync(TEntity entity)
        {
            try
            {
                await _repository.UpdateAsync(entity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating {EntityName}");
                throw new ApiException($"Failed to update {EntityName}.", ex, _logger);
            }
        }

        public virtual async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting {EntityName} with ID {id}");
                throw new ApiException($"Failed to delete {EntityName} with ID {id}.", ex, _logger);
            }
        }

        private async Task SaveChangesAsync()
        {
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving changes for {EntityName}");
                throw new ApiException($"Failed to save changes for {EntityName}.", ex, _logger);
            }
        }
    }
}