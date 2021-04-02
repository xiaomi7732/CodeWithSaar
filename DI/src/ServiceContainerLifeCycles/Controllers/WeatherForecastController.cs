using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceContainerLifeCycles.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly LifetimeInspector _lifetimeInspector;
        private readonly LifetimeInspector _lifetimeInspector1;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            LifetimeInspector lifetimeInspector,
            LifetimeInspector lifetimeInspector1,
            ILogger<WeatherForecastController> logger)
        {
            _lifetimeInspector = lifetimeInspector ?? throw new ArgumentNullException(nameof(lifetimeInspector));
            _lifetimeInspector1 = lifetimeInspector1 ?? throw new ArgumentNullException(nameof(lifetimeInspector1));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _lifetimeInspector.Call();
            _lifetimeInspector1.Call();

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
