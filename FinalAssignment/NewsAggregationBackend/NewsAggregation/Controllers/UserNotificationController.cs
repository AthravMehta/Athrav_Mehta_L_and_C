using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;
using NewsAggregation.Enums;
using NewsAggregation.Configurations;
using NewsAggregation.Models;

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
        public async Task<IActionResult> GetAllNotifications()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var notifications = await _service.GetAllUserNotificationAsync();
                return Ok(notifications);
            }, _logger);
        }

        [HttpPost]
        public async Task<IActionResult> AddNotification([FromBody] UserNotificationDto notificationDto)
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

                var addedNotification = await _service.AddAsync(notificationDto);
                return CreatedAtAction(nameof(AddNotification), new { id = addedNotification.UserNotificationId }, addedNotification);
            }, _logger);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] UserNotificationDto notificationDto)
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

                var updatedNotification = await _service.UpdateAsync(id, notificationDto);
                if (updatedNotification == null)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(updatedNotification);
            }, _logger);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                await _service.DeleteAsync(id);
                return Ok(new { Message = SuccessConstants.CategoryHidden });
            }, _logger);
        }
    }
}