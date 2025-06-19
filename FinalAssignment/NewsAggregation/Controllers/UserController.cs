using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
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
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                UserDataWithTokenDto userDataWithToken = await _userService.CreateUserAsync(userDto);
                return Ok(userDataWithToken);
            }
            catch (ApiException apiEx)
            {
                _logger.LogError(apiEx, "API error during user creation");
                return BadRequest(new { error = apiEx.Message, code = apiEx.ErrorCode.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user creation");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}
