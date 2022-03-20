using Microsoft.Extensions.Logging;

namespace CodeWithSaar.CustomLogger;

public class MyLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => new MemoryStream();

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel,
                            EventId eventId,
                            TState state,
                            Exception? exception,
                            Func<TState, Exception?, string> formatter)
    {
        // Is it possible to avoid hard code the file name?
        string fileName = "output.log";
        using (Stream outputStream = File.Open(fileName, FileMode.Append))
        using (StreamWriter streamWriter = new StreamWriter(outputStream))
        {
            // Uncomment this to see what will be output
            // streamWriter.Write($"[{logLevel}] ");
            streamWriter.WriteLine(formatter(state, exception));
        }
    }
}