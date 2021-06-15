using System;
using Microsoft.Extensions.DependencyInjection;

namespace ExtensionMethods
{
    class Program
    {
        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            
            // From 1
            ExtensionMethods.AddJWTAuth(services);
            // Form 2
            services.AddJWTAuth();
        }
    }

    static class ExtensionMethods
    {
        public static IServiceCollection AddJWTAuth(this IServiceCollection services)
        {
            // add services into the service collection;
            return services;
        }
    }
}
