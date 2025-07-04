namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleReadTrackingRepository
    {
        Task<List<int>> GetCategoriesByUserReadSequenceAsync(int userId);
    }
}
