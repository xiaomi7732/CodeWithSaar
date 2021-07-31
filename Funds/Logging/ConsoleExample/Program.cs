using System;
using Microsoft.Extensions.Logging;

namespace ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // 1. Create a logger factory, register logging providers
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            // 2. Get a logger from the logger factory
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            // 3. Logging
            logger.LogInformation("Hello Logger!");

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(intercept: true);
        }
    }
}
