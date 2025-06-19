using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Configurations.DatabaseConfigurations;
using NewsAggregation.Entities;
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

        public async Task<IEnumerable<ArticleDto>> GetAllAsync(DateTime startDate, DateTime endDate)
        {
            var userId = _requestContext.UserId;
            var query = _context.Articles
                .Where(a => a.PublishedDate >= startDate && a.PublishedDate <= endDate);

            //query = query.Where(a => a.UserArticleActions.Any(ua => ua.UserId == userId));

            var articleDtos = await query
                .ProjectTo<ArticleDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return articleDtos;
        }
    }
}