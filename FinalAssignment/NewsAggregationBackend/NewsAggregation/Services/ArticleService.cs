using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.NewsAggregation.Entities;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

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
        private int userId => _requestContext.UserId!.Value;

        public async Task<ArticleDto> AddAsync(ArticleDto dto)
        {
            var entity = _mapper.Map<Article>(dto);
            await _curdbaseRepository.AddAsync(entity);
            await _curdbaseRepository.SaveChangesAsync();
            dto.ArticleId = entity.ArticleId;
            return dto;
        }

        public async Task<IEnumerable<Article>> AddAllArticlesAsync(IEnumerable<Article> articles)
        {
            var articlesToAdd = new List<Article>();

            foreach (var article in articles)
            {
                if (!await this.ArticleExistsAsync(article))
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
            await LogUserArticleReadAsync(articleWithUserDetail);
            return articleWithUserDetail;
        }

        private async Task LogUserArticleReadAsync(ArticleDetailsDto article)
        {
            var readTrack = new UserArticleReadTracking
            {
                UserId = _requestContext.UserId!.Value,
                ArticleId = article.ArticleId,
                CategoryId = article.CategoryId
            };

            await _userArticleReadTrackingRepository.AddAsync(readTrack);
            await _curdbaseRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(ArticleQueryDto query)
        {
            var articles = await _articleRepository.GetAllAsync(query);
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<List<ArticleDto>> GetRecommendedArticlesAsync(int maxResults = 20)
        {
            var likedCategories = await _userArticleActionRepository.GetCategoriesByUserReactionAsync(userId, ReactionEnum.Like);
            var dislikedCategories = await _userArticleActionRepository.GetCategoriesByUserReactionAsync(userId, ReactionEnum.Dislike);
            var savedCategories = await _userArticleActionRepository.GetCategoriesBySavedUserArticleAsync(userId);
            var readCategories = await _customUserArticleReadTrackingRepository.GetCategoriesByUserReadSequenceAsync(userId);
            var userNotificationConfigurationCategories = await _userNotificationRepository.GetCategoriesByUserNotificationsAsync(userId);
            var userNotificationKeywordsByCategory = await _userNotificationRepository.GetKeywordsByUserNotificationsAsync(userId);
            var reportedArticleCategoryIds = await _userArticleActionRepository.GetReportedArticleCategoriesByUserAsync(userId);

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
                return new List<ArticleDto>();

            int topCategoryId = topCategoryEntry.Key;

            ArticleQueryDto query = new ArticleQueryDto
            {
                CategoryId = topCategoryId,
                IsHidden = false
            };

            var candidateArticles = await this.GetAllAsync(query);

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
            var userId = _requestContext.UserId;
            var savedArticleIds = await _userArticleActionService.GetSavedArticleIdsByUserIdAsync();
            var articles = await _articleRepository.GetArticlesByIdsAsync(savedArticleIds);
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
        }

        public async Task<bool> ArticleExistsAsync(Article article)
        {
            return await _articleRepository.ArticleExistsAsync(article);
        }

        public async Task<bool> HideArticleAsync(int articleId)
        {
            var article = await _articleRepository.GetArticleByIdAsync(articleId);
            if (article == null) return false;
            article.IsHidden = true;
            article.HideReason = HideReasonEnum.HideByAdmin;
            await _articleRepository.UpdateArticle(article);
            await _curdbaseRepository.SaveChangesAsync();
            return true;
        }

        public async Task HideArticlesByCategoryAsync(ArticleQueryDto query)
        {
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
            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                if (await _userArticleActionService.GetReportCountForArticleAsync(article.ArticleId) == 0)
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
                throw new ArgumentException("Keyword not found", nameof(keywordId));

            var query = new ArticleQueryDto
            {
                CategoryId = keyword.CategoryId
            };
            IEnumerable<Article> articles = await _articleRepository.GetAllAsync(query);

            foreach (var article in articles)
            {
                bool containsKeyword = false;
                if (!string.IsNullOrEmpty(article.Title) && article.Title.IndexOf(keyword.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    containsKeyword = true;
                else if (!string.IsNullOrEmpty(article.Content) && article.Content.IndexOf(keyword.Keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                    containsKeyword = true;

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
                throw new ArgumentException("Keyword not found", nameof(keywordId));

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
