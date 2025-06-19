using AutoMapper;
using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ICrudBaseRepository<Article> _curdbaseRepository;
        private readonly IArticleRepository _articleRepository;
        private readonly ILogger<ArticleService> _logger;
        private readonly IMapper _mapper;
            
        public ArticleService(ICrudBaseRepository<Article> repo, IArticleRepository articleRepository, ILogger<ArticleService> logger, IMapper mapper)
        {
            _curdbaseRepository = repo;
            _articleRepository = articleRepository;
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

        public async Task AddAllArticlesAsync(IEnumerable<Article> articles)
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
                try
                {
                    await _curdbaseRepository.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to save articles");
                    throw;
                }
            }
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var entity = await _curdbaseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return _mapper.Map<ArticleDto>(entity);
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate)
        {
            var entities = await _articleRepository.GetAllAsync(startDate, endDate);

            return entities;
        }

        public async Task<bool> ArticleExistsAsync(Article article)
        {
            return await _articleRepository.ArticleExistsAsync(article);
        }
    }
}
