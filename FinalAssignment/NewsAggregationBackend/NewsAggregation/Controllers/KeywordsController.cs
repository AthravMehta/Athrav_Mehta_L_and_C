using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IKeywordService _keywordsService;
        private readonly ICrudBaseService<Keywords> _crudBaseService;

        public KeywordsController(IKeywordService keywordsService, ICrudBaseService<Keywords> crudBaseService, IMapper mapper)
        {
            _keywordsService = keywordsService;
            _crudBaseService = crudBaseService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> GetAllKeywords()
        {
            var keywords = await _keywordsService.GetAllKeywordsAsync();
            var keywordDtos = _mapper.Map<List<KeywordDto>>(keywords);
            return Ok(keywordDtos);
        }

        [HttpPost]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> AddKeywords([FromBody] List<KeywordDto> keywords)
        {
            if (keywords == null || !keywords.Any())
                return BadRequest(new MessageResponseDto { IsSuccess = false, Message = "No keywords provided." });

            await _crudBaseService.AddRangeAsync(_mapper.Map<List<Keywords>>(keywords));
            return Ok(new MessageResponseDto { IsSuccess = true, Message = "Keywords Added Successfully"});
        }


        [HttpPost("hide/{keywordId}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideKeyword([FromRoute] int keywordId, [FromBody] string reason)
        {
            var result = await _keywordsService.HideKeywordAsync(keywordId, reason);
            if (!result)
                return NotFound(new MessageResponseDto { Message = "Keyword not found for the given ID." });
            return Ok(new MessageResponseDto { Message = "Keyword hidden successfully." });
        }

        [HttpPost("unhide/{keywordId}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UnhideKeyword([FromRoute] int keywordId)
        {
            var result = await _keywordsService.UnhideKeywordAsync(keywordId);
            if (!result)
                return NotFound(new MessageResponseDto { Message = "Keyword not found for the given ID." });
            return Ok(new MessageResponseDto  { Message = "Keyword unhidden successfully." });
        }
    }
}
