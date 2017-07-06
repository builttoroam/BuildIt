using System.Collections.Generic;

namespace BuildIt
{
    /// <summary>
    /// Defines methods for a configuration manager
    /// </summary>
    /// <typeparam name="TConfigurationKey">The type (enum) that defines the configuration environments</typeparam>
    /// <typeparam name="TConfiguration">The type of the configuration entity (used for configuration values for each environment)</typeparam>
    public interface IConfigurationManager<TConfigurationKey, TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Gets returns the current configuration
        /// </summary>
        TConfiguration Current { get; }

        /// <summary>
        /// Populates the configuration manager
        /// </summary>
        /// <param name="configurations">Dictionary of key (each environment) and configuration entity</param>
        void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations);

        /// <summary>
        /// Selects a configuration
        /// </summary>
        /// <param name="key">The configuration environment</param>
        void SelectConfiguration(TConfigurationKey key);
    }
}