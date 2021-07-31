using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                ["LogLevel:Default"] = "Information",
                ["LogLevel:CodeWithSaar"] = "Warning",
                ["LogLevel:CodeWithSaar.JWT"] = "Debug",
            }).Build();

            // 1. Create a logger factory, register logging providers
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConfiguration(configuration);
                builder.AddConsole();
                builder.AddDebug();
            });

            // 2. Get a logger from the logger factory
            ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

            // 3. Logging
            logger.LogDebug("Hello Debug info.");
            logger.LogInformation("Hello Logger!");
            
            ILogger logger2 = loggerFactory.CreateLogger("CodeWithSaar.JWTAuthentication.JWT");
            logger2.LogInformation("From category2");

            Console.WriteLine("Press any key to continue");
            Console.ReadKey(intercept: true);
        }
    }
}
