using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

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
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                var message = string.Format(ErrorMessages.EntityGetAllFailed, EntityName);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
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
                var message = string.Format(ErrorMessages.EntityGetByIdFailed, EntityName, id);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
            }
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                var message = ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject);
                _logger.LogWarning(message);
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, message);
            }

            try
            {
                await _repository.AddAsync(entity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = string.Format(ErrorMessages.EntityAddFailed, EntityName);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
            }
        }

        public virtual async Task AddRangeAsync(List<TEntity> entities)
        {
            if (entities == null)
            {
                var message = ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject);
                _logger.LogWarning(message);
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, message);
            }

            try
            {
                await _repository.AddRangeAsync(entities);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = string.Format(ErrorMessages.EntityAddRangeFailed, EntityName);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                var message = ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject);
                _logger.LogWarning(message);
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, message);
            }

            try
            {
                await _repository.UpdateAsync(entity);
                await SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = string.Format(ErrorMessages.EntityUpdateFailed, EntityName);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
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
                var message = string.Format(ErrorMessages.EntityDeleteFailed, EntityName, id);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
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
                var message = string.Format(ErrorMessages.EntitySaveChangesFailed, EntityName);
                _logger.LogError(ex, message);
                throw new ApiException(ErrorResponse.ErrorEnum.DatabaseError, message);
            }
        }
    }
}
