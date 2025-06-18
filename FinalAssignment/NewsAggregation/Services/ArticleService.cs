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
            
        public ArticleService(ICrudBaseRepository<Article> repo, IArticleRepository articleRepository)
        {
            _curdbaseRepository = repo;
            _articleRepository = articleRepository;
        }

        public async Task<ArticleDto> AddAsync(ArticleDto dto)
        {
            var entity = new Article
            {
                Title = dto.Title,
                Content = dto.Content,
                ExternalServerId = dto.ExternalServerId,
                CategoryId = dto.CategoryId,
                Source = dto.Source,
                Url = dto.Url,
                PublishedDate = dto.PublishedDate
            };
            await _curdbaseRepository.AddAsync(entity);
            await _curdbaseRepository.SaveChangesAsync();
            dto.Id = entity.ArticleId;
            return dto;
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var entity = await _curdbaseRepository.GetByIdAsync(id);
            if (entity == null) return null;

            return new ArticleDto
            {
                Id = entity.ArticleId,
                Title = entity.Title,
                Content = entity.Content,
                ExternalServerId = entity.ExternalServerId,
                CategoryId = entity.CategoryId,
                Source = entity.Source,
                Url = entity.Url,
                PublishedDate = entity.PublishedDate
            };
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate)
        {
            var entities = await _articleRepository.GetAllAsync(startDate, endDate);

            return entities;
        }
    }
}
