using System.Reflection;
using System.Text.Json;

namespace UserSettingsDemo;

public class ConfigurationWriter
{
    private readonly string _userSettingsFilePath;

    public ConfigurationWriter()
    {
        _userSettingsFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, UserConfiguration.FileName);
    }

    public async Task WriteConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken)
    {
        using (Stream output = new FileStream(_userSettingsFilePath, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(output, configuration);
        }
    }
}