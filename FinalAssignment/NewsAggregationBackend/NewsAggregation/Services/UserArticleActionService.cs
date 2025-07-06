using Hangfire;
using Microsoft.Extensions.FileProviders;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Notifications;
using NewsAggregation.Notifications.Contracts;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class UserArticleActionService : IUserArticleActionService
    {
        private readonly IFileProvider _fileProvider;
        private readonly IUserRepository _userRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IUserArticleActionRepository _userArticleActionRepository;
        private readonly INotificationSenderFactory _notificationSenderFactory;
        private readonly RequestContext _requestContext;

        public UserArticleActionService(
            IFileProvider fileProvider,
            IUserRepository userRepository,
            IArticleRepository articleRepository,
            IUserArticleActionRepository userArticleActionRepository,
            INotificationSenderFactory notificationSenderFactory,
            RequestContext requestContext)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _articleRepository = articleRepository ?? throw new ArgumentNullException(nameof(articleRepository));
            _userArticleActionRepository = userArticleActionRepository ?? throw new ArgumentNullException(nameof(userArticleActionRepository));
            _notificationSenderFactory = notificationSenderFactory ?? throw new ArgumentNullException(nameof(notificationSenderFactory));
            _requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
        }

        protected virtual int UserId => _requestContext.UserId!.Value;

        public async Task<ToggleSaveResponseDto> ToggleSaveAsync(int articleId)
        {
            var result = await _userArticleActionRepository.ToggleSaveAsync(UserId, articleId);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> AddArticleReaction(ArticleReactionRequestDto articleReactionRequestDto)
        {
            if (articleReactionRequestDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            var result = await _userArticleActionRepository.AddArticleReaction(UserId, articleReactionRequestDto);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteArticleReaction(int articleId)
        {
            var result = await _userArticleActionRepository.DeleteArticleReaction(UserId, articleId);
            await _userArticleActionRepository.SaveChangesAsync();
            return result;
        }

        public async Task<IEnumerable<int>> GetSavedArticleIdsByUserIdAsync()
        {
            return await _userArticleActionRepository.GetSavedArticleIdsByUserIdAsync(UserId);
        }

        public async Task<int> GetReportCountForArticleAsync(int articleId)
        {
            return await _userArticleActionRepository.GetReportCountForArticleAsync(articleId);
        }

        public async Task<UserArticleReportResponseDto> ReportArticleAsync(UserArticleReportDto userArticleReportDto)
        {
            if (userArticleReportDto == null)
                throw new ApiException(ErrorResponse.ErrorEnum.NullObject, ErrorResponse.GetErrorMessage(ErrorResponse.ErrorEnum.NullObject));

            int articleId = userArticleReportDto.ArticleId;
            if (await _userArticleActionRepository.HasUserReportedArticleAsync(articleId, UserId))
            {
                return new UserArticleReportResponseDto
                {
                    Success = true,
                    Message = SuccessConstants.ArticleAlreadyReported
                };
            }

            await _userArticleActionRepository.AddReportAsync(new UserArticleReport
            {
                ArticleId = articleId,
                ReportReason = userArticleReportDto.ReportReason,
                UserId = UserId,
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

            BackgroundJob.Enqueue(() => NotifyAdminArticleReportedWrapper(
                articleId,
                article!.Title,
                article!.Content,
                article!.PublishedDate
            ));

            return new UserArticleReportResponseDto
            {
                Success = true,
                Message = SuccessConstants.ArticleReported
            };
        }

        public void NotifyAdminArticleReportedWrapper(int articleId, string title, string content, DateTime publishedDate)
        {
            NotifyAdminArticleReported(articleId, title, content, publishedDate).Wait();
        }

        private async Task<bool> NotifyAdminArticleReported(int articleId, string title, string content, DateTime publishedDate)
        {
            var admins = await _userRepository.GetAllUsersAsync(RoleEnum.Admin);

            if (admins == null || !admins.Any())
                return false;

            string messageBody = CreateReportMessageBody(articleId, title, content, publishedDate);

            var sender = _notificationSenderFactory.GetSender(NotificationType.Email);

            var tasks = admins.Select(admin =>
                sender.SendAsync(admin.Email, AppConstants.ArticleReportEmailSubject, messageBody));

            await Task.WhenAll(tasks);

            return true;
        }

        private string CreateReportMessageBody(int articleId, string title, string content, DateTime publishedDate)
        {
            var templateFile = _fileProvider.GetFileInfo("Templates/Email/ArticleReportedTemplate.html");
            string template;
            using (var stream = templateFile.CreateReadStream())
            using (var reader = new StreamReader(stream))
            {
                template = reader.ReadToEnd();
            }

            template = template.Replace("{{ArticleId}}", articleId.ToString())
                               .Replace("{{Title}}", title)
                               .Replace("{{PublishedDate}}", publishedDate.ToString("yyyy-MM-dd HH:mm"))
                               .Replace("{{Content}}", content);

            return template;
        }
    }
}
