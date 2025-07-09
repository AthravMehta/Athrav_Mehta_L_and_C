namespace NewsAggregation.Constants
{
    public static class AppConstants
    {
        public const int ReportThreshold = 3;

        // JWT
        public const string JwtSection = "Jwt";
        public const string JwtIssuer = "Issuer";
        public const string JwtAudience = "Audience";
        public const string JwtSecretKey = "SecretKey";

        // Connection Strings
        public const string DatabaseConnection = "DatabaseConnection";

        // Email Settings
        public const string EmailSettingsSection = "EmailSettings";

        // Logging
        public const string LogsFolder = "Logs";

        // NEWS API
        public const string NewsApi = "NewsAPI";
        public const string NewsApiBase = "newsapi.org";
        public const string TheNewsApiBase = "thenewsapi.com";

        // Encryption
        public const string KeyProtector = "KeyProtector";

        // Email
        public const string UserArticleEmailSubject = "Your Personalized News Digest";
        public const string ArticleReportEmailSubject = "Article Reported Notification";

        // Validation
        public const string PasswordValidationRegex = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\W).{8,}$";
        public const string EmailValidationRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public const string NewsAggregationAppVersion = "NewsAggregationApp/1.0";

    }
}
