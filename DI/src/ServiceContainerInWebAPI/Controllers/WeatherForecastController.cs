using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DI.ServiceContainerBasics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ServiceContainerInWebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DogReport dogReport;

        public WeatherForecastController(DogReport dogReport, ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
            this.dogReport = dogReport ?? throw new ArgumentNullException(nameof(dogReport));
        }

        [HttpGet]
        public IActionResult Get()
        {
            string json = dogReport.SerializeDog(new Dog() { Name = "Vivi" });
            return Ok(json);
            
            // var rng = new Random();
            // return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            // {
            //     Date = DateTime.Now.AddDays(index),
            //     TemperatureC = rng.Next(-20, 55),
            //     Summary = Summaries[rng.Next(Summaries.Length)]
            // })
            // .ToArray();
        }
    }
}
