using AutoMapper;
using NewsAggregation.Configurations;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUserArticleActionService _userArticleActionService;
        private readonly ICrudBaseRepository<Article> _curdbaseRepository;
        private readonly ICrudBaseRepository<Keywords> _keywordsRepository;
        private readonly ICrudBaseRepository<UserArticleReadTracking> _userArticleReadTrackingRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly IUserArticleActionRepository _userArticleActionRepository;
        private readonly IUserArticleReadTrackingRepository _customUserArticleReadTrackingRepository;
        private readonly IUserNotificationRepository _userNotificationRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly RequestContext _requestContext;
        private readonly IMapper _mapper;

        private int UserId => _requestContext.UserId!.Value;

        public ArticleService(
            IUserArticleActionService userArticleActionService,
            ICrudBaseRepository<Article> crudBaseRepository,
            ICrudBaseRepository<Keywords> keywordsRepository,
            ICrudBaseRepository<UserArticleReadTracking> userArticleReadTrackingRepository,
            IUserArticleReadTrackingRepository customUserArticleReadTrackingRepository,
            IArticleRepository articleRepository,
            IUserArticleActionRepository userArticleActionRepository,
            IUserNotificationRepository userNotificationRepository,
            ILogger<ArticleService> logger,
            RequestContext requestContext,
            IMapper mapper)
        {
            _userArticleActionService = userArticleActionService;
            _curdbaseRepository = crudBaseRepository;
            _keywordsRepository = keywordsRepository;
            _userArticleReadTrackingRepository = userArticleReadTrackingRepository;
            _articleRepository = articleRepository;
            _userArticleActionRepository = userArticleActionRepository;
            _customUserArticleReadTrackingRepository = customUserArticleReadTrackingRepository;
            _userNotificationRepository = userNotificationRepository;
            _requestContext = requestContext;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ArticleDto> AddAsync(ArticleDto dto)
        {
            if (dto == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            var entity = _mapper.Map<Article>(dto);
            await _curdbaseRepository.AddAsync(entity);
            await _curdbaseRepository.SaveChangesAsync();
            dto.ArticleId = entity.ArticleId;
            return dto;
        }

        public async Task<IEnumerable<Article>> AddAllArticlesAsync(IEnumerable<Article> articles)
        {
            if (articles == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            var articlesToAdd = new List<Article>();
            foreach (var article in articles)
            {
                if (!await ArticleExistsAsync(article))
                {
                    articlesToAdd.Add(article);
                }
            }

            if (articlesToAdd.Any())
            {
                await _articleRepository.AddRangeAsync(articlesToAdd);
                await _curdbaseRepository.SaveChangesAsync();
                return articlesToAdd;
            }
            return Enumerable.Empty<Article>();
        }

        public async Task<ArticleDetailsDto> GetByIdWithUserDetailsAsync(int id)
        {
            var articleWithUserDetail = await _articleRepository.GetArticleWithUserStatusAsync(id);
            if (articleWithUserDetail == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NotFound;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            await LogUserArticleReadAsync(articleWithUserDetail);
            return articleWithUserDetail;
        }

        /// <summary>
        /// Logs which Article User is Reading.
        /// </summary>
        /// <param name="article"></param>
        /// <returns></returns>

        private async Task LogUserArticleReadAsync(ArticleDetailsDto article)
        {
            if (article == null)
            {
                _logger.LogWarning(ErrorMessages.NullObjectError);
                return;
            }

            var readTrack = new UserArticleReadTracking
            {
                UserId = UserId,
                ArticleId = article.ArticleId,
                CategoryId = article.CategoryId
            };

            await _userArticleReadTrackingRepository.AddAsync(readTrack);
            await _curdbaseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(ArticleQueryDto query)
        {
            if (query == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            var articles = await _articleRepository.GetAllAsync(query);
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<List<ArticleDto>> GetRecommendedArticlesAsync(int maxResults = 20)
        {
            var likedCategories = await _userArticleActionRepository.GetCategoriesByUserReactionAsync(UserId, ReactionEnum.Like);
            var dislikedCategories = await _userArticleActionRepository.GetCategoriesByUserReactionAsync(UserId, ReactionEnum.Dislike);
            var savedCategories = await _userArticleActionRepository.GetCategoriesBySavedUserArticleAsync(UserId);
            var readCategories = await _customUserArticleReadTrackingRepository.GetCategoriesByUserReadSequenceAsync(UserId);
            var userNotificationConfigurationCategories = await _userNotificationRepository.GetCategoriesByUserNotificationsAsync(UserId);
            var userNotificationKeywordsByCategory = await _userNotificationRepository.GetKeywordsByUserNotificationsAsync(UserId);
            var reportedArticleCategoryIds = await _userArticleActionRepository.GetReportedArticleCategoriesByUserAsync(UserId);

            var categoryScores = new Dictionary<int, double>();
            void AddScore(IEnumerable<int> categories, double weight)
            {
                foreach (var catId in categories)
                    categoryScores[catId] = categoryScores.GetValueOrDefault(catId) + weight;
            }

            AddScore(likedCategories, 5.0);
            AddScore(savedCategories, 4.0);
            AddScore(readCategories, 3.0);
            AddScore(userNotificationConfigurationCategories, 2.0);
            AddScore(reportedArticleCategoryIds, -1.0);
            AddScore(dislikedCategories, -10.0);

            var topCategoryEntry = categoryScores
                .Where(kv => kv.Value > 0)
                .OrderByDescending(kv => kv.Value)
                .FirstOrDefault();

            if (topCategoryEntry.Equals(default(KeyValuePair<int, double>)))
            {
                _logger.LogInformation(SuccessConstants.NoRecommendationFound);
                return new List<ArticleDto>();
            }

            int topCategoryId = topCategoryEntry.Key;

            var query = new ArticleQueryDto
            {
                CategoryId = topCategoryId,
                IsHidden = false
            };

            var candidateArticles = await GetAllAsync(query);

            var userKeywords = userNotificationKeywordsByCategory
                .Where(k => k.CategoryId == topCategoryId)
                .Select(k => k.Keyword)
                .ToList();

            var scoredArticles = candidateArticles
                .Select(article =>
                {
                    int matchedKeywordCount = userKeywords.Count(keyword => ArticleContainsKeyword(article, keyword));
                    return new { Article = article, Score = matchedKeywordCount };
                });

            var sortedArticles = scoredArticles
                .OrderByDescending(x => x.Score)
                .Take(maxResults)
                .Select(x => x.Article)
                .ToList();

            return sortedArticles;
        }

        /// <summary>
        /// Checks if the article contains the keyword in its title or content.
        /// </summary>
        private bool ArticleContainsKeyword(ArticleDto article, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword) || article == null)
                return false;

            var comparison = StringComparison.OrdinalIgnoreCase;

            return (article.Title?.IndexOf(keyword, comparison) >= 0)
                || (article.Content?.IndexOf(keyword, comparison) >= 0);
        }

        public async Task<IEnumerable<ArticleDto>> GetSavedArticlesForCurrentUserAsync()
        {
            var savedArticleIds = await _userArticleActionService.GetSavedArticleIdsByUserIdAsync();
            var articles = await _articleRepository.GetArticlesByIdsAsync(savedArticleIds);
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<bool> ArticleExistsAsync(Article article)
        {
            if (article == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }
            return await _articleRepository.ArticleExistsAsync(article);
        }

        public async Task<bool> HideArticleAsync(int articleId)
        {
            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (article == null)
            {
                _logger.LogWarning(ErrorMessages.ResourceNotFound);
                return false;
            }
            article.IsHidden = true;
            article.HideReason = HideReasonEnum.HideByAdmin;
            await _articleRepository.UpdateArticle(article);
            await _curdbaseRepository.SaveChangesAsync();
            return true;
        }

        public async Task HideArticlesByCategoryAsync(ArticleQueryDto query)
        {
            if (query == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                article.IsHidden = true;
                article.HideReason = HideReasonEnum.AdminHiddenCategory;
                await _curdbaseRepository.UpdateAsync(article);
            }
            await _curdbaseRepository.SaveChangesAsync();
        }

        public async Task UnhideArticlesByCategoryAsync(ArticleQueryDto query)
        {
            if (query == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NullObject;
                var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                int reportCount = await _userArticleActionService.GetReportCountForArticleAsync(article.ArticleId);
                if (reportCount == 0)
                {
                    article.IsHidden = false;
                    article.HideReason = HideReasonEnum.NotHidden;
                }
                else
                {
                    article.HideReason = HideReasonEnum.ReportLimitExceeded;
                }
                await _curdbaseRepository.UpdateAsync(article);
            }
            await _curdbaseRepository.SaveChangesAsync();
        }

        public async Task HideArticlesByKeywordAsync(int keywordId)
        {
            var keyword = await _keywordsRepository.GetByIdAsync(keywordId);
            if (keyword == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NotFound;
                var errorMessage = ErrorMessages.ResourceNotFound;
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            var query = new ArticleQueryDto
            {
                CategoryId = keyword.CategoryId
            };
            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                bool containsKeyword = ArticleContainsKeyword(_mapper.Map<ArticleDto>(article), keyword.Keyword);

                if (containsKeyword && !article.IsHidden)
                {
                    article.IsHidden = true;
                    article.HideReason = HideReasonEnum.AdminHiddenKeyword;
                    await _curdbaseRepository.UpdateAsync(article);
                }
            }
            await _curdbaseRepository.SaveChangesAsync();
        }

        public async Task UnhideArticlesByKeywordAsync(int keywordId)
        {
            var keyword = await _keywordsRepository.GetByIdAsync(keywordId);
            if (keyword == null)
            {
                var errorCode = ErrorResponse.ErrorEnum.NotFound;
                var errorMessage = ErrorMessages.ResourceNotFound;
                _logger.LogWarning(errorMessage);
                throw new ApiException(errorCode, errorMessage);
            }

            var query = new ArticleQueryDto
            {
                CategoryId = keyword.CategoryId,
                IsHidden = true,
                HideReason = HideReasonEnum.AdminHiddenKeyword
            };
            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                int reportCount = await _userArticleActionService.GetReportCountForArticleAsync(article.ArticleId);
                if (reportCount == 0)
                {
                    article.IsHidden = false;
                    article.HideReason = HideReasonEnum.NotHidden;
                }
                else
                {
                    article.HideReason = HideReasonEnum.ReportLimitExceeded;
                }
                await _curdbaseRepository.UpdateAsync(article);
            }
            await _curdbaseRepository.SaveChangesAsync();
        }
    }
}
