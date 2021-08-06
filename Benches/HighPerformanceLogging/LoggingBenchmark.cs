using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;

namespace HighPerformanceLogging
{
    public class LoggingBenchmark
    {
        private ILogger _logger;
        public LoggingBenchmark()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddConsole();
            });

            _logger = loggerFactory.CreateLogger<LoggingBenchmark>();
        }

        [Benchmark]
        public bool Logging()
        {
            _logger.LogInformation(eventId: 1, Extensions.LoggingMessageFormat, CreateARandomString());
            return true;
        }

        [Benchmark]
        public bool HighPerfLogging()
        {
            Extensions._highPerfLogging.Invoke(_logger, CreateARandomString(), default);
            return true;
        }

        [Benchmark]
        public bool HighPerfLoggingWithExtensionMethod()
        {
            _logger.LogHighPerfInfo(CreateARandomString());
            return true;
        }

        private string CreateARandomString() => Guid.NewGuid().ToString("D");
    }
}