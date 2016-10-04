using System;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Utilities;

namespace BuildIt.Config.Core.Services
{
    public class AppConfigurationServiceSetup : IAppConfigurationServiceSetup
    {
        public TimeSpan CacheExpirationTime { get; set; } = new TimeSpan(0, 0, 5);
        public AppConfigurationMode Mode { get; set; } = AppConfigurationMode.OnlyOnline;
    }
}
