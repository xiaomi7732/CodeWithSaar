using System.Text.Json;

namespace UserSettingsDemo;

public class ConfigurationWriter
{
    public async Task WriteConfigurationAsync(UserConfiguration configuration, CancellationToken cancellationToken)
    {
        using (Stream output = new FileStream(UserConfiguration.FileName, FileMode.Create, FileAccess.Write))
        {
            await JsonSerializer.SerializeAsync(output, configuration);
        }
    }
}