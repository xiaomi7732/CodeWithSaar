using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard.Biz;

public interface IBizFish
{
    Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken);
}