using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard.DataAccess;

internal interface IFishData
{
    Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken);
}