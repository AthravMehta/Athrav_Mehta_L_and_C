using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly IKeywordService _keywordsService;

        public KeywordsController(IKeywordService keywordsService)
        {
            _keywordsService = keywordsService;
        }

        [HttpPost("hide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideKeyword([FromQuery] int keywordId, [FromBody] string reason)
        {
            var result = await _keywordsService.HideKeywordAsync(keywordId, reason);
            if (!result)
                return NotFound(new MessageResponseDto { Message = "Keyword not found for the given ID." });
            return Ok(new MessageResponseDto { Message = "Keyword hidden successfully." });
        }

        [HttpPost("unhide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UnhideKeyword([FromQuery] int keywordId)
        {
            var result = await _keywordsService.UnhideKeywordAsync(keywordId);
            if (!result)
                return NotFound(new MessageResponseDto { Message = "Keyword not found for the given ID." });
            return Ok(new MessageResponseDto  { Message = "Keyword unhidden successfully." });
        }
    }
}
