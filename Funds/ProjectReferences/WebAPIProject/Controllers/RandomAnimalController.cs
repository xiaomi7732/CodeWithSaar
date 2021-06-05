using AnimalSuppliesClassLib;
using Microsoft.AspNetCore.Mvc;

namespace WebAPIProject
{
    [ApiController]
    [Route("[controller]")]
    public class RandomAnimalController : ControllerBase
    {
        private readonly AnimalProvider _animalProvider;

        public RandomAnimalController(AnimalProvider animalProvider)
        {
            _animalProvider = animalProvider ?? throw new System.ArgumentNullException(nameof(animalProvider));
        }

        [HttpGet]
        [Route("")]
        public ActionResult<string> Get()
        {
            return Ok(_animalProvider.GetAnAnimal());
        }
    }
}