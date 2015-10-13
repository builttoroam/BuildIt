using System.Collections.Generic;

namespace BuildIt
{
    public class ConfigurationManager<TConfigurationKey, TConfiguration> : IConfigurationManager<TConfigurationKey,TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration :BaseConfiguration
    {
        private IDictionary<TConfigurationKey, TConfiguration> Configurations { get; } = new Dictionary<TConfigurationKey, TConfiguration>();


        public void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations)
        {
            configurations?.DoForEach(d=>Configurations[d.Key]=d.Value);
        }

        public void SelectConfiguration(TConfigurationKey key)
        {
            Current = Configurations.SafeDictionaryValue<TConfigurationKey, TConfiguration, TConfiguration>(key);
        }

        public TConfiguration Current { get; private set; }

    }
}