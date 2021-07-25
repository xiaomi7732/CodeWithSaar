using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace ConsoleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("settings.json", optional: false);
#if DEBUG
            configurationBuilder.AddJsonFile("settings.debug.json", optional: true);
#endif
            configurationBuilder.AddEnvironmentVariables(prefix: "Demo_");

            configurationBuilder.AddUserSecrets<Program>();

            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                ["Settings1"] = "value1",
                ["ConnectionStrings:SQL"] = "FromInMemory",
            });

            IConfigurationRoot configuration = configurationBuilder.Build();
            configuration.GetSection("KnownAnimals");

            foreach ((string key, string value)
                in configuration.AsEnumerable().Where(t => t.Value is not null))
            {
                Console.WriteLine($"{key}={value}");
            }
        }
    }
}
