namespace NewsAggregation.Utilities
{
    public static class ErrorResponse
    {
        public enum ErrorEnum
        {
            BadRequest = 400,
            UnauthorizedError = 401,
            NotFound = 404,
            ConflictError = 409,
            ContentTypeError = 415,
            DatabaseError = 422,
            TooEarly = 425,
            NullObject = 700,
            Validation = 701,
            Duplicate = 702,
            FileNameNotFound = 709
        };

        private static readonly Dictionary<ErrorEnum, string> ErrorDict = new Dictionary<ErrorEnum, string>
        {
            {
                ErrorEnum.NullObject, "Model cannot be null"
            },
            {
                ErrorEnum.Validation, "Validation failed for the payload"
            },
            {
                ErrorEnum.Duplicate, "Duplicate Entry is not allowed"
            },
            {
                ErrorEnum.NotFound, "Records not found"
            },
            {
                ErrorEnum.BadRequest, "Error occurred while processing request"
            },
            {
                ErrorEnum.ConflictError, "Unable to process request due to a data conflict"
            },
            {
                ErrorEnum.ContentTypeError, "An error occurred while getting file"
            },
            {
                ErrorEnum.DatabaseError, "A data processing error occurred"
            },
            {
                ErrorEnum.UnauthorizedError, "Unauthorized Access"
            },
            {
                ErrorEnum.TooEarly, "Result Unavailable - Processing In Progress"
            },
        };

        public static string GetErrorMessage(ErrorEnum errorCode)
        {
            return ErrorDict[errorCode];
        }

        private static string GetErrorCode(ErrorEnum errorCode)
        {
            return ((int)errorCode).ToString();
        }

        public static string ToErrorCodeString(this ErrorEnum errorCode)
        {
            return GetErrorCode(errorCode);
        }
    }
}
