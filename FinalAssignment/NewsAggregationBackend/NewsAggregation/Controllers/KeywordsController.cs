using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Constants;
using NewsAggregation.Entities;
using NewsAggregation.Enums;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.Admin))]
    public class KeywordsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IKeywordService _keywordService;
        private readonly ICrudBaseService<Keywords> _crudBaseService;
        private readonly ILogger<KeywordsController> _logger;

        public KeywordsController(
            IKeywordService keywordService,
            ICrudBaseService<Keywords> crudBaseService,
            IMapper mapper,
            ILogger<KeywordsController> logger)
        {
            _keywordService = keywordService;
            _crudBaseService = crudBaseService;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllKeywords()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var keywords = await _keywordService.GetAllKeywordsAsync();
                var keywordDtos = _mapper.Map<List<KeywordDto>>(keywords);
                return Ok(keywordDtos);
            }, _logger);
        }

        [HttpPost]
        public async Task<IActionResult> AddKeywords([FromBody] List<KeywordDto> keywordDtos)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                if (keywordDtos == null || !keywordDtos.Any())
                {
                    var errorCode = ErrorResponse.ErrorEnum.BadRequest;
                    var errorMessage = ErrorMessages.InvalidModelState;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                await _crudBaseService.AddRangeAsync(_mapper.Map<List<Keywords>>(keywordDtos));
                return Ok(new MessageResponseDto { IsSuccess = true, Message = SuccessConstants.KeywordsAdded });
            }, _logger);
        }

        [HttpPost("hide/{keywordId}")]
        public async Task<IActionResult> HideKeyword([FromRoute] int keywordId, [FromBody] string reason)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _keywordService.HideKeywordAsync(keywordId, reason);
                if (!result)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(new MessageResponseDto { Message = SuccessConstants.KeywordHidden});
            }, _logger);
        }

        [HttpPost("unhide/{keywordId}")]
        public async Task<IActionResult> UnhideKeyword([FromRoute] int keywordId)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _keywordService.UnhideKeywordAsync(keywordId);
                if (!result)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(new MessageResponseDto { Message = SuccessConstants.KeywordUnhidden});
            }, _logger);
        }
    }
}