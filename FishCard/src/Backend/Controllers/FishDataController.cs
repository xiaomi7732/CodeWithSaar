using CodeWithSaar.FishCard.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeWithSaar.FishCard.Controllers;

[ApiController]
[Route("[controller]")]
public class FishDataController
{
    public Task<IReadOnlyCollection<FishItem>> GetAllFishesAsync()
    {
        
    }
}