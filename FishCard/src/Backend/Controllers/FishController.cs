using CodeWithSaar.FishCard.Biz;
using CodeWithSaar.FishCard.Models;
using Microsoft.AspNetCore.Mvc;

namespace CodeWithSaar.FishCard.Controllers;

[ApiController]
[Route("[controller]")]
public class FishController
{
    private readonly IBizFish _bizFish;

    public FishController(IBizFish bizFish)
    {
        _bizFish = bizFish ?? throw new ArgumentNullException(nameof(bizFish));
    }

    [HttpGet]
    public Task<IEnumerable<FishItem>> GetAllFishesAsync(CancellationToken cancellationToken)
        => _bizFish.GetAllFishAsync(cancellationToken);
}