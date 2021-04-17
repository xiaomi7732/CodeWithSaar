using DI.ServiceContainerBasics;
using Microsoft.AspNetCore.Mvc;

namespace Patterns
{
    [ApiController]
    [Route("[controller]")]
    public class DogReportController : ControllerBase
    {
        private readonly DogReport dogReport;

        public DogReportController(DogReport dogReport)
        {
            this.dogReport = dogReport ?? throw new System.ArgumentNullException(nameof(dogReport));
        }

        [HttpGet]
        public IActionResult PrintDog()
        {
            dogReport.Print(new Dog{
                Name = "Bella",
            });

            return Ok();
        }
    }
}
