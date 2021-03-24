using System;
using Microsoft.Extensions.DependencyInjection;

namespace DI.ServiceContainerBasics
{
    class Program
    {
        static void Main(string[] args)
        {
            // Adding the package of Microsoft.Extensions.DependencyInjection: dotnet add package Microsoft.Extensions.DependencyInjection

            // Crate a service container
            IServiceCollection serviceCollection = new ServiceCollection();

            // Register a service
            // TODO:

            // Resolve a service

            // Inject a service
        }
    }
}
