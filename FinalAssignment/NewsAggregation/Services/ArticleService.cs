using NewsAggregation.Entities;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Services
{
    public class ArticleService : IArticleService
    {
        private readonly ICrudBaseRepository<Article, int> _repo;

        public ArticleService(ICrudBaseRepository<Article, int> repo)
        {
            _repo = repo;
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
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task<ArticleDto> UpdateAsync(int id, ArticleDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.Title = dto.Title;
            entity.Content = dto.Content;
            entity.ExternalServerId = dto.ExternalServerId;
            entity.CategoryId = dto.CategoryId;
            entity.Source = dto.Source;
            entity.Url = dto.Url;
            entity.PublishedDate = dto.PublishedDate;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            dto.Id = entity.Id;
            return dto;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity != null)
            {
                _repo.Delete(entity);
                await _repo.SaveChangesAsync();
            }
        }

        public async Task<ArticleDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            return new ArticleDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                ExternalServerId = entity.ExternalServerId,
                CategoryId = entity.CategoryId,
                Source = entity.Source,
                Url = entity.Url,
                PublishedDate = entity.PublishedDate
            };
        }

        public async Task<IEnumerable<ArticleDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();

            return entities.Select(entity => new ArticleDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Content = entity.Content,
                ExternalServerId = entity.ExternalServerId,
                CategoryId = entity.CategoryId,
                Source = entity.Source,
                Url = entity.Url,
                PublishedDate = entity.PublishedDate
            });
        }

        private IQueryable<Article> ApplySorting(IQueryable<Article> query, string sortBy, bool ascending)
        {
            switch (sortBy.ToLower())
            {
                case "title":
                    query = ascending ? query.OrderBy(x => x.Title) : query.OrderByDescending(x => x.Title);
                    break;
                case "publisheddate":
                    query = ascending ? query.OrderBy(x => x.PublishedDate) : query.OrderByDescending(x => x.PublishedDate);
                    break;
                default:
                    query = ascending ? query.OrderBy(x => x.Id) : query.OrderByDescending(x => x.Id);
                    break;
            }
            return query;
        }
    }
}
