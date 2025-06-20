using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NewsAggregation.Exceptions;
using NewsAggregation.Utilities;
using System.Text.Json;

namespace NewsAggregation.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                await WriteErrorResponse(context, (int)ex.ErrorCode, ex.Message, ex.ErrorCode.ToErrorCodeString());
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error");
                var errCode = ErrorResponse.ErrorEnum.DatabaseError;
                await WriteErrorResponse(context, (int)errCode, ErrorResponse.GetErrorMessage(errCode), errCode.ToErrorCodeString());
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error");
                var errCode = ErrorResponse.ErrorEnum.DatabaseError;
                await WriteErrorResponse(context, (int)errCode, ErrorResponse.GetErrorMessage(errCode), errCode.ToErrorCodeString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                var errCode = ErrorResponse.ErrorEnum.InternalServerError;
                await WriteErrorResponse(context, (int)errCode, ErrorResponse.GetErrorMessage(errCode), errCode.ToErrorCodeString());
            }
        }

        private async Task WriteErrorResponse(HttpContext context, int statusCode, string message, string code)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            var errorObj = new { error = message, code = code };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorObj));
        }
    }
}