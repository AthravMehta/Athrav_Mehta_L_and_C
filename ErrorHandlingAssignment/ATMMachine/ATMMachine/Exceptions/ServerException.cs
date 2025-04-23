using ATMMachine.Constants;

namespace ATMMachine.Exceptions
{
    public class ServerException : Exception
    {
        public ServerException(string message) : base(message) { }
    }

    public class UnableToConnectToServerException : ServerException
    {
        public UnableToConnectToServerException() : base(ErrorConstant.UnableToConnectToServer) {}
    }
}
