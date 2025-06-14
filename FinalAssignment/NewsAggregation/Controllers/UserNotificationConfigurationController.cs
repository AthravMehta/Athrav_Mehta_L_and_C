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
    public class UserNotificationConfigurationController : ControllerBase
    {
        private readonly ILogger<UserNotificationConfigurationController> _logger;
        private readonly IUserNotificationConfigurationService _service;

        public UserNotificationConfigurationController(ILogger<UserNotificationConfigurationController> logger, IUserNotificationConfigurationService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] UserNotificationConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserNotificationConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
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
        public async Task<IActionResult> GetAll([FromQuery] Guid? userId)
        {
            var result = await _service.GetAllAsync(userId);
            return Ok(result);
        }
    }
}
