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
        [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
        public async Task<IActionResult> UpdateNotificationConfiguration(int id, [FromBody] UserNotificationConfigurationDto dto)
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

                var result = await _service.UpdateAsync(id, dto);
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

        [HttpGet]
        [AuthorizeRoles(nameof(RoleEnum.Admin), nameof(RoleEnum.User))]
        public async Task<IActionResult> GetAllNotificationConfigurations()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _service.GetAllUserConfigurationAsync();
                return Ok(result);
            }, _logger);
        }

        [HttpPost("initialize")]
        [AuthorizeRoles(nameof(RoleEnum.Admin))]
        public async Task<IActionResult> InitializeNotificationConfigurations()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                await _service.InitializeNotificationConfigurationsAsync();
                return Ok(new { Message = SuccessConstants.UNCInitialize});
            }, _logger);
        }
    }
}