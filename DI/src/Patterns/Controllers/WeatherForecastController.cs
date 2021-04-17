using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI.ServiceContainerBasics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Patterns.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        private readonly DogReport _dogReport;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            DogReport dogReport,
            ILogger<WeatherForecastController> logger)
        {
            _dogReport = dogReport ?? throw new ArgumentNullException(nameof(dogReport));
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            _dogReport.Print(new Dog()
            {
                Name = "Bella",
            });
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
