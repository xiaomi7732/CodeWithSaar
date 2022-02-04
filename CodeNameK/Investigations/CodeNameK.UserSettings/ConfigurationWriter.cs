using System.Reflection;
using System.Text.Json;

namespace UserSettingsDemo;

public class ConfigurationWriter
{
    private readonly string _userSettingsFilePath;
    private readonly JsonSerializerOptions _serializationOptions;

    public ConfigurationWriter()
    {
        _userSettingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, UserConfiguration.FileName);
        _serializationOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        _serializationOptions.WriteIndented = true;
        _serializationOptions.Converters.Add(new SectionInfoJsonConverter<UserConfiguration>());
    }

    public async Task WriteConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken)
    {
        using (Stream output = new FileStream(_userSettingsFilePath, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(output, new SectionInfo<UserConfiguration>(nameof(UserConfiguration), configuration), _serializationOptions).ConfigureAwait(false);
        }
    }
}