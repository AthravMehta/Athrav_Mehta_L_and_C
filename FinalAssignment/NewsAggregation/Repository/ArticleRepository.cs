using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;

namespace NewsAggregation.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly NewsAggregationDbContext _context;
        private readonly IMapper _mapper;
        private readonly RequestContext _requestContext;

        public ArticleRepository(NewsAggregationDbContext context, IMapper mapper, RequestContext requestContext)
        {
            _context = context;
            _mapper = mapper;
            _requestContext = requestContext;
        }

        public async Task<IEnumerable<Article>> GetAllAsync(ArticleQueryDto articleQueryDto)
        {
            var articleQuery = _context.Articles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(articleQueryDto.SearchText))
            {
                articleQuery = articleQuery.Where(a =>
                    a.Title.Contains(articleQueryDto.SearchText) ||
                    a.Content.Contains(articleQueryDto.SearchText));
            }

            if (articleQueryDto.StartDate.HasValue)
            {
                articleQuery = articleQuery.Where(a => a.PublishedDate >= articleQueryDto.StartDate.Value);
            }

            if (articleQueryDto.EndDate.HasValue)
            {
                articleQuery = articleQuery.Where(a => a.PublishedDate <= articleQueryDto.EndDate.Value);
            }

            if (articleQueryDto.CategoryId.HasValue && articleQueryDto.CategoryId.Value > 0)
            {
                articleQuery = articleQuery.Where(a => a.CategoryId == articleQueryDto.CategoryId.Value);
            }

            if (articleQueryDto.SortByLikes)
            {
                articleQuery = articleQuery.OrderByDescending(a => a.UserArticleReactions.Count(r => r.Reaction == ReactionEnum.Like));
            }
            else if (articleQueryDto.SortByDislikes)
            {
                articleQuery = articleQuery.OrderByDescending(a => a.UserArticleReactions.Count(r => r.Reaction == ReactionEnum.Dislike));
            }
            else
            {
                articleQuery = articleQuery.OrderByDescending(a => a.PublishedDate);
            }

            return await articleQuery
                .Include(a => a.UserArticleReactions)
                .ToListAsync();
        }
        public async Task<IEnumerable<Article>> GetArticlesByIdsAsync(IEnumerable<int> articleIds)
        {
            if (articleIds == null || !articleIds.Any())
                return Enumerable.Empty<Article>();

            return await _context.Articles
                .Where(a => articleIds.Contains(a.ArticleId))
                .ToListAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Article> articles)
        {
            await _context.Articles.AddRangeAsync(articles);
        }

        public async Task<bool> ArticleExistsAsync(Article article)
        {
            return await _context.Articles.AnyAsync(a => a.Url == article.Url);
        }
    }
}