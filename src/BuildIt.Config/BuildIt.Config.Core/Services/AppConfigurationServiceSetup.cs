using System;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Utilities;

namespace BuildIt.Config.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationServiceSetup : IAppConfigurationServiceSetup
    {
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan CacheExpirationTime { get; set; } = new TimeSpan(0, 0, 5);
        /// <summary>
        /// 
        /// </summary>
        public AppConfigurationMode Mode { get; set; } = AppConfigurationMode.OnlyOnline;
    }
}
