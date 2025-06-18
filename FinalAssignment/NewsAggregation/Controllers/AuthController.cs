using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                UserDataWithTokenDto userDataWithToken = await _authService.LoginAsync(userDto);
                return Ok(userDataWithToken);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex.Message);
                return Unauthorized(new { error = ex.Message });
            }
        }
    }
}
