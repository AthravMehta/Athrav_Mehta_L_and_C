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

       public UserArticleActionService(
           IUserRepository userRepository,
           IArticleRepository articleRepository, 
           IUserArticleActionRepository userArticleActionRepository, 
           NotificationSenderFactory notificationSenderFactory,
           RequestContext requestContext)
        {
            _userRepository = userRepository;
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

        public async Task<UserArticleReportResponseDto> ReportArticleAsync(UserArticleReportDto userArticleReportDto)
        {
            UserArticleReportResponseDto responseDto = new UserArticleReportResponseDto
            {
                Success = false,
                Message = string.Empty
            };
            int articleId = userArticleReportDto.ArticleId;
            if (await _userArticleActionRepository.HasUserReportedArticleAsync(articleId, userId))
            {
                responseDto.Success = true;
                responseDto.Message = "You have already reported this article.";
                return responseDto;
            }

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

            // TODO: Figure out a way to less the params, since sending whole article object is causing error
            BackgroundJob.Enqueue(() => NotifyAdminArticleReportedWrapper(
                articleId,
                article.Title,
                article.Content,
                article.PublishedDate
            ));

            responseDto.Success = true;
            responseDto.Message = "Article reported successfully. Thank you for your feedback!";
            return responseDto;
        }

        public void NotifyAdminArticleReportedWrapper(int articleId, string title, string content, DateTime publishedDate)
        {
            NotifyAdminArticleReported(articleId, title, content, publishedDate).Wait();
        }

        private async Task<bool> NotifyAdminArticleReported(int articleId,
            string title,
            string content,
            DateTime publishedDate)
        {
            var admins = await _userRepository.GetAllUsersAsync(RoleEnum.Admin);

            if (admins == null || !admins.Any())
                return false;

            string messageBody = this.CreateReportMessageBody(articleId, title, content, publishedDate);

            var sender = _notificationSenderFactory.GetSender(NotificationType.Email);

            var tasks = admins.Select(admin =>
                sender.SendAsync(admin.Email, "Article Reported Notification", messageBody));

            await Task.WhenAll(tasks);

            return true;
        }

        private string CreateReportMessageBody(int articleId,
            string title,
            string content,
            DateTime publishedDate)
        {
            return $@"
                <h3>Article Reported</h3>
                <p>The article with ID <strong>{articleId}</strong> has been reported by users.</p>
                <p><strong>Title:</strong> {title}</p>
                <p><strong>Published Date:</strong> {publishedDate:yyyy-MM-dd HH:mm}</p>
                <p><strong>Description:</strong> {content}</p>
                <p>Please review the article and take necessary action.</p>
            ";
        }
    }

}
