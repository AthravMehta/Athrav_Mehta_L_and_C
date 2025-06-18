namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleActionRepository
    {
        Task<bool> ToggleSaveAsync(int userId, int articleId);
    }

}
