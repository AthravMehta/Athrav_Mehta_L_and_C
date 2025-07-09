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
    [Route("api/[controller]")]
    [ApiController]
    [AuthorizeRoles(nameof(RoleEnum.Admin))]
    public class ExternalServerController : ControllerBase
    {
        private readonly ILogger<ExternalServerController> _logger;
        private readonly IExternalServerService _externalServerService;

        public ExternalServerController(ILogger<ExternalServerController> logger, IExternalServerService externalServerService)
        {
            _logger = logger;
            _externalServerService = externalServerService;
        }

        [HttpPost]
        public async Task<IActionResult> AddExternalServer([FromBody] ExternalServerDto externalServerDto)
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

                var result = await _externalServerService.AddAsync(externalServerDto);
                return CreatedAtAction(nameof(GetExternalServerById), new { id = result.ExternalServerId }, result);
            }, _logger);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExternalServer(int id, [FromBody] ExternalServerDto externalServerDto)
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

                var result = await _externalServerService.UpdateAsync(id, externalServerDto);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExternalServerById(int id)
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _externalServerService.GetByIdAsync(id);
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
        public async Task<IActionResult> GetAllExternalServers()
        {
            return await RequestHandler.HandleRequestAsync(async () =>
            {
                var result = await _externalServerService.GetAllAsync();
                return Ok(result);
            }, _logger);
        }
    }
}