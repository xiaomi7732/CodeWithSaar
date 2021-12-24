using Microsoft.Extensions.DependencyInjection;
using System;

namespace CodeNameK.ViewModels
{
    internal class ErrorRevealerFactory : IErrorRevealerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ErrorRevealerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public IErrorRevealer CreateInstance(string? title = null)
        {
            return ActivatorUtilities.CreateInstance<ErrorRevealer>(_serviceProvider, string.IsNullOrEmpty(title) ? string.Empty : title);
        }
    }
}
