using System.Text.Json;
using CodeWithSaar.FishCard.Models;

namespace CodeWithSaar.FishCard.DataAccess;

internal class FishData : IFishData
{
    private readonly string _dataFilePath;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public FishData(
        string dataFilePath,
        JsonSerializerOptions jsonSerializerOptions)
    {
        _dataFilePath = dataFilePath ?? throw new ArgumentNullException(nameof(dataFilePath));
        _jsonSerializerOptions = jsonSerializerOptions ?? throw new ArgumentNullException(nameof(jsonSerializerOptions));
    }

    public async Task<IEnumerable<FishItem>> GetAllFishAsync(CancellationToken cancellationToken)
    {
        using FileStream dataFile = File.OpenRead(_dataFilePath);
        IEnumerable<FishItem>? allFishData = await JsonSerializer.DeserializeAsync<IEnumerable<FishItem>>(dataFile, _jsonSerializerOptions).ConfigureAwait(false);
        if (allFishData is null)
        {
            return Enumerable.Empty<FishItem>();
        }
        return allFishData.ToList();
    }
}