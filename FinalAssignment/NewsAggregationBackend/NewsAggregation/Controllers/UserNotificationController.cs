using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.User))]
    public class UserNotificationController : ControllerBase
    {
        private readonly ILogger<UserNotificationController> _logger;
        private readonly IUserNotificationService _service;

        public UserNotificationController(ILogger<UserNotificationController> logger, IUserNotificationService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllUserNotificationAsync();
            return Ok(result);
        }
    }
}
