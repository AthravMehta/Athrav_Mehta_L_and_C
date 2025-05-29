using SingeltonLoggingService.Services;

namespace SingeltonLoggingService
{
    class Program
    {
        public static void Main(string[] args)
        {
            Logger.Instance.LogInfo("Application Started");

            TestLoggerInMultithreadedEnvironment();

            Logger.Instance.LogInfo("Application Exited");
        }

        static void TestLoggerInMultithreadedEnvironment()
        {
            Console.WriteLine("Starting multi-threaded logger test.");

            Parallel.For(0, 10, i =>
            {
                Logger.Instance.LogInfo($"Log from thread {i}");
            });

            Console.WriteLine("All threads have finished logging.");
        }
    }
}