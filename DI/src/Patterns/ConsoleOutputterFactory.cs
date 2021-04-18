using Microsoft.Extensions.DependencyInjection;

namespace DI.ServiceContainerBasics
{
    public class ConsoleOutputterFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsoleOutputterFactory(IServiceScopeFactory scopeFactory)
        {
            this._scopeFactory = scopeFactory ?? throw new System.ArgumentNullException(nameof(scopeFactory));
        }

        public IOutputter Create()
        {
            // return new ConsoleOutputter();

            // This way, console outputter is still constructed by the service container:
            return _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<ConsoleOutputter>();
        }
    }
}