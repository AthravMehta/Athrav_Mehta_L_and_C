using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
    public class UserKeywordController : ControllerBase
    {
        private readonly ILogger<UserKeywordController> _logger;
        private readonly IUserKeywordService _userKeywordService;

        public UserKeywordController(ILogger<UserKeywordController> logger, IUserKeywordService userKeywordService)
        {
            _logger = logger;
            _userKeywordService = userKeywordService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserKeywordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userKeywordService.AddAsync(dto);
            return CreatedAtAction(nameof(Add), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userKeywordService.GetAllAsync();
            return Ok(result);
        }
    }
}
