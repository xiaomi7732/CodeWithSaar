using System.Net.Http.Json;
using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard;

internal class QueryFishService
{
    private readonly BackendClient _backendClient;
    private FishItem[]? _allFish = null;

    public QueryFishService(BackendClient backendClient)
    {
        _backendClient = backendClient ?? throw new System.ArgumentNullException(nameof(backendClient));
    }

    /// <summary>
    /// Gets a list of fish items by keyword. The keyword needs to at least partially match the fish name. It is case insensitive.
    /// </summary>
    public async Task<IEnumerable<FishItem>> GetFishesByAsync(string? keyword, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Enumerable.Empty<FishItem>();
        }

        keyword = keyword.Trim();

        return (await GetAllFishIntoCacheAsync(cancellationToken)).Where(f => f.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all the fish items.
    /// </summary>
    public Task<IEnumerable<FishItem>> GetAllFishesAsync(CancellationToken cancellationToken)
    {
        return GetAllFishIntoCacheAsync(cancellationToken);
    }

    private async Task<IEnumerable<FishItem>> GetAllFishIntoCacheAsync(CancellationToken cancellationToken)
    {
        if (_allFish is null)
        {
            _allFish = (await _backendClient.GetAllFishAsync(cancellationToken)).ToArray();
        }

        return _allFish;
    }
}