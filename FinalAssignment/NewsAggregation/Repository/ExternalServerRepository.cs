using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class ExternalServerRepository : IExternalServerRepository
    {
        private readonly NewsAggregationDbContext _dbContext;

        public ExternalServerRepository(NewsAggregationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ExternalServer>> GetAllAsync(bool? isActiveFilter = null)
        {
            IQueryable<ExternalServer> query = _dbContext.ExternalServers.AsNoTracking();

            if (isActiveFilter.HasValue)
            {
                query = query.Where(es => es.IsActive == isActiveFilter.Value);
            }

            return await query.ToListAsync();
        }
    }
}
