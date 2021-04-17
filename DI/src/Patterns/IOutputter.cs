using System;
using Microsoft.Extensions.DependencyInjection;

namespace DI.ServiceContainerBasics
{
    public interface IOutputter : IDisposable
    {
        void WriteLine(string value);
    }

    public class ConsoleOutputter : IOutputter
    {
        Guid _id = Guid.NewGuid();

        public void Dispose()
        {
            Console.WriteLine($"{nameof(IOutputter)} Dispose is called.");
        }

        public void WriteLine(string value)
        {
            Console.WriteLine(_id);
            Console.WriteLine(value);
        }
    }

    // public class ConsoleOutputterFactory
    // {
    //     private readonly IServiceScopeFactory scopeFactory;

    //     public ConsoleOutputterFactory(IServiceScopeFactory scopeFactory)
    //     {
    //         this.scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
    //     }

    //     public ConsoleOutputter Create()
    //     {
    //         return scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ConsoleOutputter>();
    //     }
    // }

    // public class ScopedServiceFactory<T>
    //     where T: IDisposable
    // {
    //     private readonly IServiceScopeFactory scopeFactory;

    //     public ScopedServiceFactory(IServiceScopeFactory scopeFactory)
    //     {
    //         this.scopeFactory = scopeFactory;
    //     }

    //     public T Create()
    //         => scopeFactory.CreateScope().ServiceProvider.GetRequiredService<T>();
    // }
}