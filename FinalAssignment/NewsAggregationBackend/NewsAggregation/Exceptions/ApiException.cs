using NewsAggregation.Utilities;

namespace NewsAggregation.Exceptions
{
    public class ApiException : Exception
    {
        public ErrorResponse.ErrorEnum ErrorCode { get; set; }

        public ApiException(string details, Exception ex = null, ILogger logger = null) : base(details, ex)
        {
            if(logger != null)
            {
                logger.LogError(Message);
            }
            ErrorCode = ErrorResponse.ErrorEnum.BadRequest;
        }
        
        public ApiException(ErrorResponse.ErrorEnum errCode ,string details = null, Exception ex = null, ILogger logger = null) : base(ErrorResponse.GetErrorMessage(errCode) + " : " + details, ex)
        {
            if(logger != null)
            {
                logger.LogError(Message);
            }
            ErrorCode = ErrorResponse.ErrorEnum.BadRequest;
        }
    }
}
