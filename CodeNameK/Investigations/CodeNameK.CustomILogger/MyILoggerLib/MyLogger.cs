using Microsoft.Extensions.Logging;

namespace CodeWithSaar.CustomLogger;

public class MyLogger : ILogger
{
    private readonly string _categoryName;
    private readonly Func<MyLoggerOptions> _getOptions;

    public MyLogger(string categoryName, Func<MyLoggerOptions> getOptions)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new ArgumentException($"'{nameof(categoryName)}' cannot be null or empty.", nameof(categoryName));
        }

        _categoryName = categoryName;
        _getOptions = getOptions ?? throw new ArgumentNullException(nameof(getOptions));
    }

    public IDisposable BeginScope<TState>(TState state) => new MemoryStream();

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel,
                            EventId eventId,
                            TState state,
                            Exception? exception,
                            Func<TState, Exception?, string> formatter)
    {
        string fileName = _getOptions().OutputFilePath;
        using(Stream outputStream = File.Open(fileName, FileMode.Append))
        using(StreamWriter streamWriter = new StreamWriter(outputStream))
        {
            streamWriter.Write($"[{_categoryName}, {logLevel}] ");
            streamWriter.WriteLine(formatter(state, exception));
        }
    }
}