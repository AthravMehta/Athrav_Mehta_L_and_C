using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUserArticleActionService _userArticleActionService;
        private readonly ICrudBaseRepository<Article> _curdbaseRepository;
        private readonly ICrudBaseRepository<Keywords> _keywordsRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly RequestContext _requestContext;
        private readonly IMapper _mapper;

        public ArticleService(
            IUserArticleActionService userArticleActionService,
            ICrudBaseRepository<Article> crudBaseRepository,
            IArticleRepository articleRepository,
            ILogger<ArticleService> logger,
            RequestContext requestContext,
            IMapper mapper)
        {
            _userArticleActionService = userArticleActionService;
            _curdbaseRepository = crudBaseRepository;
            _articleRepository = articleRepository;
            _requestContext = requestContext;
            _logger = logger;
            _mapper = mapper;
        }

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
            return articleWithUserDetail;
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(ArticleQueryDto query)
        {
            var articles = await _articleRepository.GetAllAsync(query);
            return _mapper.Map<IEnumerable<ArticleDto>>(articles);
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
