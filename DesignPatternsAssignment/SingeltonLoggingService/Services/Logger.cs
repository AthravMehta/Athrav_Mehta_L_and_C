namespace SingeltonLoggingService.Services
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> _loggerInstance = new Lazy<Logger>(() => new Logger());
        private static readonly object _fileLock = new object();
        private static int _instanceCount = 0;
        private readonly string _logFilePath;

        private Logger()
        {
            Interlocked.Increment(ref _instanceCount);
            Console.WriteLine($"Logger constructor called. Instance count: {_instanceCount}");
            _logFilePath = "log.txt";
        }

        public static Logger Instance = _loggerInstance.Value;

        public void LogInfo(string message) => Log("INFO", message);
        public void LogDebug(string message) => Log("DEBUG", message);
        public void LogError(string message) => Log("ERROR", message);

        private void Log(string level, string message)
        {
            var logEntry = $"[{level}] -- {DateTime.Now} -- {message}";
            Console.WriteLine(logEntry);
            lock (_fileLock)
            {
                File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
            }
        }
    }
}
