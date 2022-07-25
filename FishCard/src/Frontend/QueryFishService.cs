using System.Net.Http.Json;
using System.Text.Json;
using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard;

internal class QueryFishService
{
    private readonly HttpClient _httpClient;
    private FishItem[]? _allFish = null;

    public QueryFishService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
    }

    /// <summary>
    /// Gets a list of fish items by keyword. The keyword needs to at least partially match the fish name. It is case insensitive.
    /// </summary>
    public async Task<IEnumerable<FishItem>> GetFishesByAsync(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return Enumerable.Empty<FishItem>();
        }

        keyword = keyword.Trim();

        return (await GetAllFishIntoCacheAsync()).Where(f => f.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Gets all the fish items.
    /// </summary>
    public Task<IEnumerable<FishItem>> GetAllFishesAsync()
    {
        return GetAllFishIntoCacheAsync();
    }

    private async Task<IEnumerable<FishItem>> GetAllFishIntoCacheAsync()
    {
        if (_allFish is null)
        {
            // JsonSerializerOptions options = new JsonSerializerOptions()
            // {
            //     PropertyNameCaseInsensitive = true,
            // };
            // options.Converters.Add(new EnumStringConverter());
            // _allFish = await _httpClient.GetFromJsonAsync<FishItem[]>("fishdata.json", options);
            _allFish = await _httpClient.GetFromJsonAsync<FishItem[]>("fishdata.json");
        }

        if (_allFish is null)
        {
            return Enumerable.Empty<FishItem>();
        }
        return _allFish;
    }
}