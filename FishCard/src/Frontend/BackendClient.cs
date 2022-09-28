using System.Net.Http.Json;
using CodeWithSaar.FishCard.Models;
using CodeWithSaar.FishCard.Utilities;

namespace CodeWithSaar.FishCard;

internal class BackendClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public BackendClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken)
    {
        Uri getFishUri = new Uri("fish", UriKind.Relative);

        FishItem[]? fishItems = null;
        using (HttpClient httpClient = _httpClientFactory.CreateClient(HttpClientName.Backend))
        {
            fishItems = await httpClient.GetFromJsonAsync<FishItem[]>(getFishUri, cancellationToken);
        }
        return fishItems.NullAsEmpty();
    }
}