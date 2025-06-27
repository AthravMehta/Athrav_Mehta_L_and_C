using Hangfire;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Notifications;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class UserArticleActionService : IUserArticleActionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IUserArticleActionRepository _userArticleActionRepository;
        private readonly NotificationSenderFactory _notificationSenderFactory;
        private readonly RequestContext _requestContext;

       public UserArticleActionService(IArticleRepository articleRepository, 
           IUserArticleActionRepository userArticleActionRepository, 
           NotificationSenderFactory notificationSenderFactory,
           RequestContext requestContext)
        {
            _articleRepository = articleRepository;
            _userArticleActionRepository = userArticleActionRepository;
            _notificationSenderFactory = notificationSenderFactory;
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

        public async Task ReportArticleAsync(UserArticleReportDto userArticleReportDto)
        {
            int articleId = userArticleReportDto.ArticleId;
            if (await _userArticleActionRepository.HasUserReportedArticleAsync(articleId, userId))
                throw new ApiException("You have already reported this article.");

            await _userArticleActionRepository.AddReportAsync(new UserArticleReport
            {
                ArticleId = articleId,
                ReportReason = userArticleReportDto.ReportReason,
                UserId = userId,
                ActionCreatedTime = DateTime.UtcNow
            });

            int reportCount = await _userArticleActionRepository.GetReportCountForArticleAsync(articleId);

            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (reportCount >= AppConstants.ReportThreshold)
            {
                if (article != null && !article.IsHidden)
                {
                    article.IsHidden = true;
                    article.HideReason = HideReasonEnum.ReportLimitExceeded;
                    await _articleRepository.UpdateArticle(article);
                }
            }

            BackgroundJob.Enqueue(() => this.NotifyAdminArticleReported(articleId, article));
        }

        public async Task<bool> NotifyAdminArticleReported(int articleId, Article article)
        {
            var admins = await _userRepository.GetAllUsersAsync(RoleEnum.Admin);

            if (admins == null || !admins.Any())
                return false;

            string messageBody = this.CreateReportMessageBody(articleId, article);

            var sender = _notificationSenderFactory.GetSender(NotificationType.Email);

            var tasks = admins.Select(admin =>
                sender.SendAsync(admin.Email, "Article Reported Notification", messageBody));

            await Task.WhenAll(tasks);

            return true;
        }

        private string CreateReportMessageBody(int articleId, Article article)
        {
            return $@"
                <h3>Article Reported</h3>
                <p>The article with ID <strong>{articleId}</strong> has been reported by users.</p>
                <p><strong>Title:</strong> {article.Title}</p>
                <p><strong>Published Date:</strong> {article.PublishedDate:yyyy-MM-dd HH:mm}</p>
                <p><strong>Description:</strong> {article.Content}</p>
                <p>Please review the article and take necessary action.</p>
            ";
        }
    }

}
