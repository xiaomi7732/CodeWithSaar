using CodeWithSaar.FishCard.DataAccess;
using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard.Biz;

internal class BizFish : IBizFish
{
    private readonly IFishData _fishData;

    public BizFish(IFishData fishData)
    {
        _fishData = fishData ?? throw new ArgumentNullException(nameof(fishData));
    }

    public Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken)
    {
        return _fishData.GetAllFishAsync(cancellationToken);
    }
}