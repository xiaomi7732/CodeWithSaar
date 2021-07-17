using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HelloOptions
{
    [ApiController]
    [Route("[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly ApplicationOptions _options;

        public MetadataController(IOptions<ApplicationOptions> options)
        {
            _options = options?.Value ?? throw new System.ArgumentNullException(nameof(options));
        }

        [HttpGet]
        public ActionResult GetOperation()
        {
            return Ok(new
            {
                ApplicationName = _options.Name,
                CacheLifeTime = _options.CacheLifetime,
                Greeting = $"Hello, my name is {_options.Name}! The settings for cache lifetime is: {_options.CacheLifetime:c}",
            });
        }
    }
}