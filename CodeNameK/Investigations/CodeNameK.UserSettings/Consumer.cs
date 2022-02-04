using Microsoft.Extensions.Options;

namespace UserSettingsDemo
{
    public class Consumer
    {
        private readonly IOptionsMonitor<UserConfiguration> _options;
        public event EventHandler<UserConfiguration>? UserConfigurationChanged;

        public Consumer(IOptionsMonitor<UserConfiguration> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _options.OnChange(newValue =>
            {
                UserConfigurationChanged?.Invoke(this, newValue);
            });
        }

        public void PrintSyncSettings()
        {
            Console.WriteLine($"Is sync enabled: {_options.CurrentValue.IsSyncEnabled}");
        }

        public UserConfiguration GetOptions()
        {
            return _options.CurrentValue;
        }
    }
}