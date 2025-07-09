namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleReadTrackingRepository
    {
        /// <summary>
        /// Get the Categories, whose article user has read.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<int>> GetCategoriesByUserReadSequenceAsync(int userId);
    }
}
