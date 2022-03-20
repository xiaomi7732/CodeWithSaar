using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeWithSaar.CustomLogger;
[ProviderAlias("FileProvider")]
public sealed class MyLoggerProvider : ILoggerProvider
{
    private MyLoggerOptions _currentOptions;
    private IDisposable? _updateToken;
    private readonly ConcurrentDictionary<string, ILogger> _loggers = 
        new ConcurrentDictionary<string, ILogger>(StringComparer.OrdinalIgnoreCase);

    public MyLoggerProvider(IOptionsMonitor<MyLoggerOptions> options)
    {
        _currentOptions = options.CurrentValue;
        _updateToken = options.OnChange(updatedOptions => _currentOptions = updatedOptions);
    }

    public ILogger CreateLogger(string categoryName)
    {
        return _loggers.GetOrAdd(categoryName, key => new MyLogger(
            categoryName,
            GetCurrentOptions));
    }

    private MyLoggerOptions GetCurrentOptions() => _currentOptions;

    public void Dispose()
    {
        _updateToken?.Dispose();
        _updateToken = null;
    }
}