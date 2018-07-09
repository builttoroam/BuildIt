using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using BuildIt;
using BuildIt.Config.Core.Extensions;
using BuildIt.Config.Core.Services;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.AR.FormsSamples.Core.Services
{
    public class ConfigurationService : IConfigurationService
    {
        public IConfigurationManager<EnvironmentConfigurations, EnvironmentConfiguration> ConfigurationManager { get; }
            = new ConfigurationManager<EnvironmentConfigurations, EnvironmentConfiguration>();

        public EnvironmentConfiguration CurrentConfiguration => ConfigurationManager.Current;

        public ConfigurationService()
        {
            ConfigurationManager.Populate(Constants.Configurations);
            ConfigurationManager.SelectConfiguration(Constants.CurrentConfiguration);
        }
    }
}
