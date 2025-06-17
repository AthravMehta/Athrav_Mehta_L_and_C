namespace NewsAggregation.Repository.Contracts
{
    public interface IUserArticleActionRepository
    {
        Task<bool> ToggleSaveAsync(Guid userId, int articleId);
    }

}
