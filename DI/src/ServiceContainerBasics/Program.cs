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
            serviceCollection.AddSingleton<IOutputter, ConsoleOutputter>();
            serviceCollection.AddSingleton<DogReport>();
            serviceCollection.AddSingleton<ISerializer, Serializer1>();
            serviceCollection.AddSingleton<ISerializer, Serializer2>();

            // Build a service provider
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

            // Resolve a service
            IOutputter outputter = serviceProvider.GetRequiredService<IOutputter>();
            DogReport dogReport = serviceProvider.GetRequiredService<DogReport>();


            // Inject a service
            outputter.WriteLine("Hello DI!");
            dogReport.Print(new Dog(){
                Name="Bella",
                Breed = "A",
                Weight=10
            });
        }
    }
}
