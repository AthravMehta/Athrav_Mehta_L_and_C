using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;
using NewsAggregation.Exceptions;
using NewsAggregation.Services;

namespace NewsAggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;
        private readonly IUserArticleActionService _userArticleActionService;

        public ArticleController(
            ILogger<ArticleController> logger,
            IArticleService service,
            IUserArticleActionService userArticleActionService)
        {
            _logger = logger;
            _service = service;
            _userArticleActionService = userArticleActionService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ArticleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ArticleId }, result);
        }

        [HttpPost("toggle-save")]
        public async Task<IActionResult> ToggleSave([FromBody] ToggleSaveRequestDto request)
        {
            var result = await _userArticleActionService.ToggleSaveAsync(request.ArticleId);
            return Ok(result);
        }

        [HttpPost("reaction")]
        public async Task<IActionResult> AddArticleReaction([FromBody] ArticleReactionRequestDto request)
        {
            var result = await _userArticleActionService.AddArticleReaction(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdWithUserDetailsAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ArticleQueryDto query)
        {
            var articles = await _service.GetAllAsync(query);
            return Ok(articles);
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetAllSavedArticles()
        {
            var savedArticles = await _service.GetSavedArticlesForCurrentUserAsync();
            return Ok(savedArticles);
        }

        [HttpDelete("reaction")]
        public async Task<IActionResult> DeleteArticleReaction([FromBody] int articleId)
        {
            var result = await _userArticleActionService.DeleteArticleReaction(articleId);
            return Ok(result);
        }

        [HttpPost("{articleId}/report")]
        public async Task<IActionResult> ReportArticle(UserArticleReportDto userArticleReportDto)
        {
            UserArticleReportResponseDto result = await _userArticleActionService.ReportArticleAsync(userArticleReportDto);
            return Ok(result);

        }

        [HttpPost("hide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideArticle([FromQuery] int articleId)
        {
            var result = await _service.HideArticleAsync(articleId);
            if (!result)
                return NotFound(new { message = "Article not found for the given ID." });
            return Ok(new { message = "Article hidden successfully." });
        }
    }
}
