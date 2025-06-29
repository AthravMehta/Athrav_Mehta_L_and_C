using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUserArticleActionService _userArticleActionService;
        private readonly ICrudBaseRepository<Article> _curdbaseRepository;
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
    }
}
