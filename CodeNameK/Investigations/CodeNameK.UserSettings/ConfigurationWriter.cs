using System.Text.Json;

namespace UserSettingsDemo;

class ConfigurationWriter
{
    private readonly JsonSerializerOptions _serializationOptions;

    public ConfigurationWriter()
    {
        _serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        _serializationOptions.WriteIndented = true;
        _serializationOptions.Converters.Add(new SectionInfoJsonConverter<UserConfiguration>());
    }

    public async Task WriteConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken)
    {
        using (Stream output = new FileStream(UserConfiguration.FilePath, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(output, new SectionInfo<UserConfiguration>(nameof(UserConfiguration), configuration), _serializationOptions).ConfigureAwait(false);
        }
    }
}