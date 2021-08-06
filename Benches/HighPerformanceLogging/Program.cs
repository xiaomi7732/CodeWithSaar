using BenchmarkDotNet.Running;

namespace HighPerformanceLogging
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<LoggingBenchmark>();
        }
    }
}
