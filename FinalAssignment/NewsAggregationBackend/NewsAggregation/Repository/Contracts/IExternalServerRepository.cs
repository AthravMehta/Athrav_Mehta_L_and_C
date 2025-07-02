using NewsAggregation.Entities;

namespace NewsAggregation.Repository.Contracts
{
    public interface IExternalServerRepository
    {
        /// <summary>
        /// Gets external servers filtered by IsActive status.
        /// If isActiveFilter is null, returns all servers.
        /// If true, returns only active servers.
        /// If false, returns only inactive servers.
        /// </summary>
        Task<IEnumerable<ExternalServer>> GetAllAsync(bool? isActiveFilter = null);
    }
}
