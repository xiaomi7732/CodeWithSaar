using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.CustomLogger;
[ProviderAlias("FileProvider")]
public sealed class MyLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = 
        new ConcurrentDictionary<string, ILogger>(StringComparer.OrdinalIgnoreCase);

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, key => new MyLogger());
    }

    public void Dispose()
    {
    }
}