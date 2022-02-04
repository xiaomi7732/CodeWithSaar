using Microsoft.Extensions.Options;

namespace UserSettingsDemo
{
    public class Consumer
    {
        private readonly IOptions<UserConfiguration> _options;

        public Consumer(IOptions<UserConfiguration> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public void PrintSyncSettings()
        {
            Console.WriteLine($"Is sync enabled: {_options.Value.IsSyncEnabled}");
        }
    }
}