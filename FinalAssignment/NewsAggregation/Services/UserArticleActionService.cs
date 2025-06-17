using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserArticleActionService : IUserArticleActionService
    {
        private readonly IUserArticleActionRepository _repo;
        private readonly RequestContext _requestContext;

        public UserArticleActionService(IUserArticleActionRepository repo, RequestContext requestContext)
        {
            _repo = repo;
            _requestContext = requestContext;
        }

        public async Task<bool> ToggleSaveAsync(int articleId)
        {
            return await _repo.ToggleSaveAsync(_requestContext.UserId.Value, articleId);
        }
    }

}
