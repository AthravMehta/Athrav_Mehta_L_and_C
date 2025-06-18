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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
