using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Controllers;
using NewsAggregation.Entities;
using NewsAggregation.Exceptions;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

[ApiController]
[Route("api/users")]
public class UserController : CrudBaseController<User, Guid>
{
    private readonly ICrudBaseService<User, Guid> _service;
    private readonly ILogger<UserController> _logger;

    public UserController(ICrudBaseService<User, Guid> service, ILogger<UserController> logger) : base(service)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserCreateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username,
                PasswordHash = HashPassword(dto.Password),
                Email = dto.Email,
                RoleId = dto.RoleId,
                CreatedDateTime = DateTime.UtcNow,
                LastUpdatedDateTime = DateTime.UtcNow
            };

            await _service.AddAsync(user);

            var resultDto = MapToReadDto(user);

            return CreatedAtAction(nameof(CreateUser), new { id = user.Id }, resultDto);
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
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _service.GetByIdAsync(id);
            if (existingUser == null)
                return NotFound();

            // Update fields if provided
            if (!string.IsNullOrWhiteSpace(dto.Username))
                existingUser.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                existingUser.PasswordHash = HashPassword(dto.Password);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                existingUser.Email = dto.Email;

            if (dto.RoleId.HasValue)
                existingUser.RoleId = dto.RoleId.Value;

            existingUser.LastUpdatedDateTime = DateTime.UtcNow;

            await _service.UpdateAsync(existingUser);

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
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var dto = MapToReadDto(user);
            return Ok(dto);
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
            var users = await _service.GetAllAsync();
            var dtos = new List<UserReadDto>();
            foreach (var user in users)
            {
                dtos.Add(MapToReadDto(user));
            }
            return Ok(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during fetching all users");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    private UserReadDto MapToReadDto(User user)
    {
        return new UserReadDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            LastLoginDateTime = user.LastLoginDateTime,
            CreatedDateTime = user.CreatedDateTime,
            LastUpdatedDateTime = user.LastUpdatedDateTime
        };
    }

    private string HashPassword(string password)
    {
        return password;
    }
}
