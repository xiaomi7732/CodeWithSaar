using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HelloOptions
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationNameController : ControllerBase
    {
        private readonly ApplicationOptions _options;

        public ApplicationNameController(IOptions<ApplicationOptions> options)
        {
            _options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
        }

        [HttpGet]
        public ActionResult GetOperation()
        {
            return Ok(new
            {
                ApplicationName = _options.Name,
                Greeting = $"Hello, my name is {_options.Name}!",
            });
        }
    }
}