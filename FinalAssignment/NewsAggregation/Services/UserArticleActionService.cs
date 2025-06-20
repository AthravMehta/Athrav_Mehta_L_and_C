using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserArticleActionService : IUserArticleActionService
    {
        private readonly IUserArticleActionRepository _userArticleActionRepository;
        private readonly RequestContext _requestContext;

        public UserArticleActionService(IUserArticleActionRepository userArticleActionRepository, RequestContext requestContext)
        {
            _userArticleActionRepository = userArticleActionRepository;
            _requestContext = requestContext;
        }

        protected virtual int userId => _requestContext.UserId!.Value;

        public async Task<ToggleSaveResponseDto> ToggleSaveAsync(int articleId)
        {
            var result = await _userArticleActionRepository.ToggleSaveAsync(userId, articleId);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> AddArticleReaction(ArticleReactionRequestDto articleReactionRequestDto)
        {
            var result = await _userArticleActionRepository.AddArticleReaction(userId, articleReactionRequestDto);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }
        public async Task<bool> DeleteArticleReaction(int articleId)
        {
            var result =  await _userArticleActionRepository.DeleteArticleReaction(userId, articleId);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync()
        {
            return await _userArticleActionRepository.GetSavedArticleIdsByUserIdAsync(userId);
        }
    }

}
