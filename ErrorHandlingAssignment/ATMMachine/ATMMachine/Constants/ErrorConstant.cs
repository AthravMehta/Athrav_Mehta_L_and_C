namespace ATMMachine.Constants
{
    public class ErrorConstant
    {
        public const string InsufficientFunds = "Insufficient funds in the account.";
        public const string UnableToConnectServer = "Unable to connect to the server.";
        public const string CardBlocked = "Your Card is Blocked. Reach Bank to Unblock";
        public const string CardBlockedAfter3FailedAttemps = "Card is blocked after 3 failed attempts.";
        public const string DailyTransactionLimitExceed = "Daily transaction limit exceeded.";
        public const string AtmDoesntHaveCash = "ATM does not have enough cash.";
        public const string AmountGreaterThanZero = "Entered Amount must be greater than 0";
        public const string InvalidNumber = "Invalid number. Please enter a valid integer.";
        public const string AccountAndCardError = "Account and Card didn't get fetched correctly. Logout and Login Again";
    }
}
