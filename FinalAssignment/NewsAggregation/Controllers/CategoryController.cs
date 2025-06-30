using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.AddAsync(categoryDto);
            return CreatedAtAction(nameof(Add), new { id = result.CategoryId }, result);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, categoryDto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }


        [HttpPost("hide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> HideCategory([FromQuery] int categoryId, [FromBody] string reason)
        {
            var result = await _categoryService.HideCategoryAsync(categoryId, reason);
            if (!result)
                return NotFound(new { message = "Category not found for the given ID." });
            return Ok(new { message = "Category hidden successfully." });
        }

        [HttpPost("unhide")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> UnhideCategory([FromQuery] int categoryId)
        {
            var result = await _categoryService.UnhideCategoryAsync(categoryId);
            if (!result)
                return NotFound(new { message = "Category not found for the given ID." });
            return Ok(new { message = "Category unhidden successfully." });
        }
    }
}
