using System;
using Microsoft.Extensions.Logging;

namespace ServiceContainerLifeCycles
{
    public sealed class LifetimeInspector : IDisposable
    {
        private Guid Id = Guid.NewGuid();
        private readonly ILogger<LifetimeInspector> _logger;

        public void Call()
        {
            _logger.LogInformation("You are calling lifetime inspector by id: {id}", Id);
        }

        public LifetimeInspector(ILogger<LifetimeInspector> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
            _logger.LogInformation("{name} created. Id: {id}", nameof(LifetimeInspector), Id);
        }

        public void Dispose()
        {
            _logger.LogInformation("{name} disposed. Id: {id}", nameof(LifetimeInspector), Id);
        }
    }
}