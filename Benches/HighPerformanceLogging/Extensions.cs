
using System;
using Microsoft.Extensions.Logging;

namespace HighPerformanceLogging
{
    internal static class Extensions
    {
        public const string LoggingMessageFormat ="Hello World! {RandomString}";
        static Extensions()
        {
            _highPerfLogging = LoggerMessage.Define<string>(LogLevel.Information, eventId: 1, LoggingMessageFormat);
        }
        internal static readonly Action<ILogger, string, Exception> _highPerfLogging;

        public static void LogHighPerfInfo(this ILogger logger, string message)
            => _highPerfLogging.Invoke(logger, message, default);
    }
}