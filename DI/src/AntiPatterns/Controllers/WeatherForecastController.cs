using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI.ServiceContainerBasics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AntiPatterns.Controllers
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
        // private readonly IOutputter _outputter;
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(
            DogReport dogReport,
            // IOutputter outputter,
            ILogger<WeatherForecastController> logger)
        {
            _dogReport = dogReport ?? throw new ArgumentNullException(nameof(dogReport));
            // _outputter = outputter ?? throw new ArgumentNullException(nameof(outputter));
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            // _outputter.WriteLine("Hello");
            _dogReport.Print(new Dog()
            {
                Name = "Bella",
                Breed = "Chihuahua",
                Weight = 10
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
