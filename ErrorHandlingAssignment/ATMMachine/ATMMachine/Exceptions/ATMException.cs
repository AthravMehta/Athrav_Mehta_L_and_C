using ATMMachine.Constants;

namespace ATMMachine.Exceptions
{
    
    public class ATMException : Exception
    {
        public ATMException(string message) : base(message) { }
    }

    public class InsufficientFundsException : ATMException
    {
        public InsufficientFundsException() : base(ErrorConstant.InsufficientFunds) { }
    }

    public class ServerConnectionException : ATMException
    {
        public ServerConnectionException() : base(ErrorConstant.UnableToConnectServer) { }
    }

    public class CardBlockedException : ATMException
    {
        public CardBlockedException() : base(ErrorConstant.CardBlockedAfter3FailedAttemps) { }
    }

    public class DailyLimitExceededException : ATMException
    {
        public DailyLimitExceededException() : base(ErrorConstant.DailyTransactionLimitExceed) { }
    }

    public class AtmRanOutOfCashException : ATMException
    {
        public AtmRanOutOfCashException() : base(ErrorConstant.AtmDoesntHaveCash) { }
    }

    public class AccountAndCardException : ATMException
    {
        public AccountAndCardException() : base(ErrorConstant.AccountAndCardError) { }
    }
}
