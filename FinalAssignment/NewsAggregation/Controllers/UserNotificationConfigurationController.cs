using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;
using NewsAggregation.Services;

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
            var result = await _service.GetAllUserConfigurationAsync();
            return Ok(result);
        }

        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        [HttpPost("Initialize")]
        public async Task<IActionResult> InitializeNotificationConfigurations()
        {
            try
            {
                await _service.InitializeNotificationConfigurationsAsync();
                return Ok(new { Message = "User notification configurations initialized successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing user notification configurations.");
                return StatusCode(500, "An error occurred while initializing configurations.");
            }
        }
    }
}
