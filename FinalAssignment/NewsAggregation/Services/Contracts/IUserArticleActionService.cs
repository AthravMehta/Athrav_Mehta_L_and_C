namespace NewsAggregation.Services.Contracts
{
    public interface IUserArticleActionService
    {
        Task<bool> ToggleSaveAsync(int articleId);
    }

}
