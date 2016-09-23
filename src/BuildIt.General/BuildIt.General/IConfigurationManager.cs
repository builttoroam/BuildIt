using System.Collections.Generic;

namespace BuildIt
{
    public interface IConfigurationManager<TConfigurationKey, TConfiguration>
        where TConfigurationKey : struct
        where TConfiguration : BaseConfiguration
    {
        void Populate(IDictionary<TConfigurationKey, TConfiguration> configurations);

        void SelectConfiguration(TConfigurationKey key);

        TConfiguration Current { get; }
    }
}