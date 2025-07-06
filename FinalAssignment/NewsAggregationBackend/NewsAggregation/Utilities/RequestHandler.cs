using Microsoft.AspNetCore.Mvc;
using NewsAggregation.Constants;
using NewsAggregation.Exceptions;

namespace NewsAggregation.Utilities
{
    public static class RequestHandler
    {
        public static async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> action, ILogger logger)
        {
            try
            {
                return await action();
            }
            catch (ApiException ex)
            {
                logger.LogError(ex, ex.Message);
                return new ObjectResult(new { error = ex.Message })
                {
                    StatusCode = (int)ex.ErrorCode
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ErrorMessages.UnexpectedError);
                return new ObjectResult(new { error = ErrorMessages.UnexpectedError })
                {
                    StatusCode = (int)ErrorResponse.ErrorEnum.InternalServerError
                };
            }
        }
    }
}