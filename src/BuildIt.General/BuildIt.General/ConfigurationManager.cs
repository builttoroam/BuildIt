using System.Collections.Generic;

namespace BuildIt
{
    /// <summary>
    /// Manages different configurations
    /// </summary>
    /// <typeparam name="TConfigurationKey">The type of the configuration environments (defined as an enum)</typeparam>
    /// <typeparam name="TConfiguration">The type of the configuration entity (which houses the configuration values for each platform)</typeparam>
    public class ConfigurationManager<TConfigurationKey, TConfiguration> : IConfigurationManager<TConfigurationKey,TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration :BaseConfiguration
    {
        private IDictionary<TConfigurationKey, TConfiguration> Configurations { get; } = new Dictionary<TConfigurationKey, TConfiguration>();


        /// <summary>
        /// Populates the configuration information based on a dictionary of environment configurations
        /// </summary>
        /// <param name="configurations">Dictionary of configuration values - key is an enum that defines the list of environments</param>
        public void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations)
        {
            configurations?.DoForEach(d=>Configurations[d.Key]=d.Value);
        }

        /// <summary>
        /// Select a specific environment
        /// </summary>
        /// <param name="key">The enum value for the specific platform</param>
        public void SelectConfiguration(TConfigurationKey key)
        {
            Current = Configurations.SafeValue<TConfigurationKey, TConfiguration, TConfiguration>(key);
        }

        /// <summary>
        /// Retrieves the currently selected configuration
        /// </summary>
        public TConfiguration Current { get; private set; }

    }
}