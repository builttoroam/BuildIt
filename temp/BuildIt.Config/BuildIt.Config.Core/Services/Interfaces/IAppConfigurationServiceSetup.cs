using System;
using System.Globalization;
using BuildIt.Config.Core.Utilities;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationServiceSetup
    {
        AppConfigurationMode Mode { get; set; }
        TimeSpan CacheExpirationTime { get; set; }
    }
}
