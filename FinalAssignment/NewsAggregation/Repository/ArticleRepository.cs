using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Repository.Contracts;
using System.Globalization;

namespace NewsAggregation.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly NewsAggregationDbContext _context;
        private readonly IMapper _mapper;
        private readonly RequestContext _requestContext;

        public ArticleRepository(
            NewsAggregationDbContext context,
            IMapper mapper,
            RequestContext requestContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(ArticleRepository));
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

            if (!string.IsNullOrWhiteSpace(articleQueryDto.StartDate) &&
        DateTime.TryParseExact(articleQueryDto.StartDate, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startDate))
            {
                articleQuery = articleQuery.Where(a => a.PublishedDate >= startDate);
            }

            if (!string.IsNullOrWhiteSpace(articleQueryDto.EndDate) &&
                DateTime.TryParseExact(articleQueryDto.EndDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endDate))
            {
                articleQuery = articleQuery.Where(a => a.PublishedDate <= endDate);
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
