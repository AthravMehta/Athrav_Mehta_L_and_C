using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;

[ApiController]
[Route("api/[controller]")]
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
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var createdUser = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedUser = await _userService.UpdateUserAsync(id, dto);
            if (updatedUser == null)
                return NotFound();

            return NoContent();
        }
        catch (ApiException apiEx)
        {
            _logger.LogError(apiEx, "API error during user update");
            return BadRequest(new { error = apiEx.Message, code = apiEx.ErrorCode.ToString() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user update");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(Guid id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during fetching user");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during fetching all users");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }
}
