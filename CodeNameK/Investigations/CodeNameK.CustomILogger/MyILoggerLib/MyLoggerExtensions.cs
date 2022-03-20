using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

namespace CodeWithSaar.CustomLogger;

public static class MyLoggerExtensions
{
    public static ILoggingBuilder AddFile(this ILoggingBuilder builder)
    {
        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, MyLoggerProvider>()
        );

        LoggerProviderOptions.RegisterProviderOptions
            <MyLoggerOptions, MyLoggerProvider>(builder.Services);

        return builder;
    }

    public static ILoggingBuilder AddFile(this ILoggingBuilder builder, Action<MyLoggerOptions> configure)
    {
        builder.AddFile();
        builder.Services.Configure(configure);
        return builder;
    }
}