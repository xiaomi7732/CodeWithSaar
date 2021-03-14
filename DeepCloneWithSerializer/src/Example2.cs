using System.Text.Json;

namespace DeepCloneWithSerializer
{
    public class Configuration
    {
        // This as a default is just a template and should always be cloned before use.
        private static Configuration _defaultConfigureTemplate = new Configuration()
        {
            CPUTrigger = new CPUTrigger() { Rate = .8 }
        };

        public CPUTrigger CPUTrigger { get; set; }

        // This one will cause trouble. See the unit test using this.
        public static Configuration Default => _defaultConfigureTemplate.ShallowCopy();


        // When asked, always return the clone of it.
        public static Configuration GoodDefault => _defaultConfigureTemplate.DeepClone();
        
        public Configuration ShallowCopy()
            => (Configuration)_defaultConfigureTemplate.MemberwiseClone();
        // Since configuration references CPU trigger, a deep clone is required.
        public Configuration DeepClone()
            => JsonSerializer.Deserialize<Configuration>(JsonSerializer.Serialize(this, this.GetType()));
    }

    public class CPUTrigger
    {
        public double Rate { get; set; }
    }
}