using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;
using NewsAggregation.Utilities;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto userDto)
        {
            return (ActionResult)await RequestHandler.HandleRequestAsync(async () =>
            {
                if (!ModelState.IsValid)
                {
                    ErrorResponse.ErrorEnum errorCode = ErrorResponse.ErrorEnum.Validation;
                    string errorMessage = ErrorResponse.GetErrorMessage(errorCode);
                    _logger.LogWarning(errorMessage);
                    throw new ApiException(errorCode, errorMessage);
                }

                UserDataWithTokenDto userDataWithToken = await _authService.LoginAsync(userDto);
                return Ok(userDataWithToken);
            }, _logger);
        }
    }
}