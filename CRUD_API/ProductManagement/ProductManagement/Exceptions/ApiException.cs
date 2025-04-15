using ProductManagement.Enums;

namespace ProductManagement.Exceptions
{
    public class ApiException : Exception
    {
        public  ErrorCode errorCode { get; }

        public ApiException(string details, Exception ex = null) : base(details, ex)
        {
            this.errorCode = ErrorCode.BadRequest;
        }

        public ApiException(ErrorCode errorCode, string details = null, Exception ex = null) : base(errorCode + ": " + details, ex)
        {
            this.errorCode = errorCode;
        }
    }
}
