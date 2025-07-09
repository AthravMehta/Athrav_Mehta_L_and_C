using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Constants;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _articleService;
        private readonly IUserArticleActionService _userArticleActionService;

        public ArticleController(
            ILogger<ArticleController> logger,
            IArticleService articleService,
            IUserArticleActionService userArticleActionService)
        {
            _logger = logger;
            _articleService = articleService;
            _userArticleActionService = userArticleActionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] ArticleDto articleDto)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    var errorCode = ErrorResponse.ErrorEnum.Validation;
                    var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                var result = await _articleService.AddAsync(articleDto);
                return CreatedAtAction(nameof(GetArticleById), new { id = result.ArticleId }, result);
            }, _logger);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(int id)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _articleService.GetByIdWithUserDetailsAsync(id);
                if (result == null)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(result);
            }, _logger);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllArticles([FromQuery] ArticleQueryDto query)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var articles = await _articleService.GetAllAsync(query);
                return Ok(articles);
            }, _logger);
        }

        [HttpPost("toggle-save")]
        public async Task<IActionResult> ToggleSaveArticle([FromBody] ToggleSaveRequestDto request)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userArticleActionService.ToggleSaveAsync(request.ArticleId);
                return Ok(result);
            }, _logger);
        }

        [HttpPost("reaction")]
        public async Task<IActionResult> AddArticleReaction([FromBody] ArticleReactionRequestDto request)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userArticleActionService.AddArticleReaction(request);
                return Ok(result);
            }, _logger);
        }

        [HttpGet("recommendation")]
        public async Task<IActionResult> GetRecommendedArticles(int count = 20)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var recommendedArticles = await _articleService.GetRecommendedArticlesAsync(count);
                return Ok(recommendedArticles);
            }, _logger);
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSavedArticles()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var savedArticles = await _articleService.GetSavedArticlesForCurrentUserAsync();
                return Ok(savedArticles);
            }, _logger);
        }

        [HttpDelete("reaction")]
        public async Task<IActionResult> DeleteArticleReaction([FromBody] int articleId)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userArticleActionService.DeleteArticleReaction(articleId);
                return Ok(result);
            }, _logger);
        }

        [HttpPost("{articleId}/report")]
        public async Task<IActionResult> ReportArticle(UserArticleReportDto userArticleReportDto)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userArticleActionService.ReportArticleAsync(userArticleReportDto);
                return Ok(result);
            }, _logger);
        }

        [HttpPost("hide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideArticle([FromQuery] int articleId)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _articleService.HideArticleAsync(articleId);
                if (!result)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(new { message = SuccessConstants.ArticleHidden});
            }, _logger);
        }
    }
}