using Microsoft.AspNetCore.Authorization;
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
    [ApiController]
    [Route("api/[controller]")]
    [AuthorizeRoles(nameof(RoleEnum.Admin))]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
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

                var userDataWithToken = await _userService.CreateUserAsync(userDto);
                return Ok(userDataWithToken);
            }, _logger);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] RoleEnum role = RoleEnum.User)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var users = await _userService.GetAllUsersAsync(role);
                return Ok(users);
            }, _logger);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetUserByName(string username)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var user = await _userService.GetUserByName(username);
                if (user == null)
                {
                    var errorCode = ErrorResponse.ErrorEnum.NotFound;
                    var errorMessage = ErrorMessages.ResourceNotFound;
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                return Ok(user);
            }, _logger);
        }
    }
}