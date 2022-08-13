using System.Net.Http.Json;
using CodeWithSaar.FishCard.Models;
using CodeWithSaar.FishCard.Utilities;

namespace CodeWithSaar.FishCard;

internal class BackendClient
{
    private readonly HttpClient _httpClient;

    public BackendClient(HttpClient backendCaller)
    {
        _httpClient = backendCaller;
    }

    public async Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken)
    {
        Uri getFishUri = new Uri("fish", UriKind.Relative);
        FishItem[]? fishItems = await _httpClient.GetFromJsonAsync<FishItem[]>(getFishUri, cancellationToken).ConfigureAwait(false);
        return fishItems.NullAsEmpty();
    }
}