namespace NewsAggregation.Constants
{
    public static class ErrorMessages
    {
        public const string InvalidModelState = "The provided model state is invalid.";
        public const string UnexpectedError = "An unexpected error occurred. Please try again later.";
        public const string UnauthorizedAccess = "Unauthorized access detected.";
        public const string ResourceNotFound = "The requested resource was not found.";
        public const string DuplicateEntry = "Duplicate entry is not allowed.";
        public const string NullObjectError = "The provided object cannot be null.";
        public const string InvalidPassword = "Password Entered by User is Invalid";
        public const string InvalidEmailFormat = "Email format is invalid.";

        public const string EntityGetAllFailed = "Failed to retrieve all {0} entities.";
        public const string EntityGetByIdFailed = "Failed to retrieve {0} with ID {1}.";
        public const string EntityAddFailed = "Failed to add new {0}.";
        public const string EntityAddRangeFailed = "Failed to add range of {0} entities.";
        public const string EntityUpdateFailed = "Failed to update {0}.";
        public const string EntityDeleteFailed = "Failed to delete {0} with ID {1}.";
        public const string EntitySaveChangesFailed = "Failed to save changes for {0}.";

        public const string UsernameValidationFailed = "Username must be at least 8 characters long.";
        public const string PasswordValidationFailed = "Password must be at least 8 characters long and contain uppercase, lowercase, and special character.";

        public const string UnsupportedApi = "Unsupported API";
    }
}
