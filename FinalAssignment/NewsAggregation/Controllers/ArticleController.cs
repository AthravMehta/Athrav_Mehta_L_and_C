using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _service;
        private readonly IUserArticleActionService _userArticleActionService;

        public ArticleController(ILogger<ArticleController> logger, IArticleService service, IUserArticleActionService userArticleActionService)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(DateTime startDate, DateTime endDate)
        {
            var result = await _service.GetAllAsync(startDate, endDate);
            return Ok(result);
        }

        [HttpPost("toggle-save")]
        public async Task<IActionResult> ToggleSave([FromBody] ToggleSaveRequestDto request)
        {
            var result = await _userArticleActionService.ToggleSaveAsync(request.ArticleId);
            return Ok(result);
        }

        // TODO: Implement this method
        [HttpPost("reaction")]
        public async Task<IActionResult> LikeDislikeArticleReaction([FromBody] ArticleReactionRequestDto request)
        {
            var result = await _userArticleActionService.ToggleSaveAsync(request.ArticleId);
            return Ok(result);
        }
    }
}
