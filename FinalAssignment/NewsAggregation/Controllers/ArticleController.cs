using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;

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
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ArticleQueryDto query)
        {
            try
            {
                var articles = await _service.GetAllAsync(query);
                return Ok(articles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching articles");
                return StatusCode(500, new { error = "Internal server error" });
            }
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
    }
}
