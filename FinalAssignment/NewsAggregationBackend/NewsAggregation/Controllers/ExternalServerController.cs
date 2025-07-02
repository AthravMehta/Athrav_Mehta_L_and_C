using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Configurations;
using NewsAggregation.Enums;
using NewsAggregation.Models;
using NewsAggregation.Services.Contracts;

namespace NewsAggregation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.Admin))]
    public class ExternalServerController : ControllerBase
    {
        private readonly ILogger<ExternalServerController> _logger;
        private readonly IExternalServerService _service;

        public ExternalServerController(ILogger<ExternalServerController> logger, IExternalServerService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] ExternalServerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ExternalServerId}, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ExternalServerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }
    }
}
