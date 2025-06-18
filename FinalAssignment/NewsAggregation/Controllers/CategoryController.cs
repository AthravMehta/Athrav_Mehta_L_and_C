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
        public async Task<IActionResult> Add([FromBody] CategoryDto dto)
        {
            // TODO: Whenever a Category gets created, in user notification configuration
            // for all User that configuration should get added with some default value
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.AddAsync(dto);
            return CreatedAtAction(nameof(Add), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryService.UpdateAsync(id, dto);
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
    }
}
