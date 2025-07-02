using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            if (articleQueryDto.IsHidden)
            {
                articleQuery = articleQuery.Where(a => a.IsHidden);

                if (articleQueryDto.HideReason  == null || articleQueryDto.HideReason != HideReasonEnum.NotHidden)
                {
                    articleQuery = articleQuery.Where(a => a.HideReason == articleQueryDto.HideReason);
                }
            }
            else
            {
                articleQuery = articleQuery.Where(a => !a.IsHidden);
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

        public async Task<ArticleDetailsDto> GetArticleWithUserStatusAsync(int articleId)
        {
            var userId = _requestContext.UserId;

            var article = await _context.Articles
                .Include(a => a.UserArticleReactions)
                .Include(a => a.UserArticleReports)
                .Include(a => a.UserSavedArticles)
                .FirstOrDefaultAsync(a => a.ArticleId == articleId);

            if (article == null) return null;

            var dto = _mapper.Map<ArticleDetailsDto>(article);

            dto.IsSavedByUser = article.UserSavedArticles.Any(usa => usa.UserId == userId);
            dto.IsReportedByUser = article.UserArticleReports.Any(uar => uar.UserId == userId);

            var userReaction = article.UserArticleReactions
                .FirstOrDefault(r => r.UserId == userId);
            dto.UserReaction = userReaction?.Reaction;

            return dto;
        }



        public async Task<Article> GetArticleByIdAsync(int articleId)
        {
            return await _context.Articles
                .FirstOrDefaultAsync(a => a.ArticleId == articleId);
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

        public async Task<bool> UpdateArticle(Article article)
        {
            _context.Update(article);
            return await this.SaveChangesAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            if (await _context.SaveChangesAsync() > 0)
                return true;
            else return false;
        }
    }
}
