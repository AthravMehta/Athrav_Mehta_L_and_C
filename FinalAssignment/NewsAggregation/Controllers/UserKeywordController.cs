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
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserKeywordDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userKeywordService.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userKeywordService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userKeywordService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userKeywordService.GetAllAsync();
            return Ok(result);
        }
    }
}
