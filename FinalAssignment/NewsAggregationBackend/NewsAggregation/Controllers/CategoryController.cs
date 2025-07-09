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
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }

        [HttpPost]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> Add([FromBody] CategoryDto categoryDto)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    ErrorResponse.ErrorEnum errorCode = ErrorResponse.ErrorEnum.Validation;
                    string errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                CategoryDto result = await _categoryService.AddAsync(categoryDto);
                return Ok(result);
            }, _logger);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    ErrorResponse.ErrorEnum errorCode = ErrorResponse.ErrorEnum.Validation;
                    string errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                var result = await _categoryService.UpdateAsync(id, categoryDto);
                if (result == null)
                {
                    _logger.LogWarning(ErrorMessages.ResourceNotFound);
                    return NotFound(new { error = ErrorMessages.ResourceNotFound });
                }

                return Ok(result);
            }, _logger);
        }

        [HttpGet]
        [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
        public async Task<IActionResult> GetAll()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _categoryService.GetAllAsync();
                return Ok(result);
            }, _logger);
        }

        [HttpPost("hide/{categoryId}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideCategory([FromRoute] int categoryId, [FromBody] string reason)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _categoryService.HideCategoryAsync(categoryId, reason);
                if (!result)
                {
                    _logger.LogWarning(ErrorMessages.ResourceNotFound);
                    return NotFound(new { error = ErrorMessages.ResourceNotFound });
                }

                return Ok(new { message = SuccessConstants.CategoryHidden});
            }, _logger);
        }

        [HttpPost("unhide/{categoryId}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UnhideCategory([FromRoute] int categoryId)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _categoryService.UnhideCategoryAsync(categoryId);
                if (!result)
                {
                    _logger.LogWarning(ErrorMessages.ResourceNotFound);
                    return NotFound(new { error = ErrorMessages.ResourceNotFound });
                }

                return Ok(new { message = SuccessConstants.CategoryUnhidden });
            }, _logger);
        }
    }
}