using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
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
        public async Task<IActionResult> AddUserKeyword([FromBody] UserKeywordDto userKeywordDto)
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

                var result = await _userKeywordService.AddAsync(userKeywordDto);
                return CreatedAtAction(nameof(AddUserKeyword), new { id = result.UserKeywordId }, result);
            }, _logger);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserKeywords()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userKeywordService.GetAllAsync();
                return Ok(result);
            }, _logger);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserKeywordById(int id)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _userKeywordService.GetByIdAsync(id);
                if (result == null)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(result);
            }, _logger);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserKeyword(int id, [FromBody] UserKeywordDto userKeywordDto)
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

                var result = await _userKeywordService.UpdateAsync(id, userKeywordDto);
                if (result == null)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(result);
            }, _logger);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserKeyword(int id)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                await _userKeywordService.DeleteAsync(id);
                return Ok(new { message = SuccessConstants.UserKeywordDeleted});
            }, _logger);
        }
    }
}