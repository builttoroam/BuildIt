using System;
using System.Threading.Tasks;
using BuildIt.Config.Core.Extensions;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Core.Services
{
    public static class ConfigurationHelpers
    {
        public static async Task<bool> CheckMinimumVersion(this IAppConfigurationService configurationService, bool retrieveCached = false)
        {
            //var isValidVersionNumber = false;

            var minVerKey = "App_VersionInfo_MinimumRequiredVersion";
            //Version appVersion;
            //Mapper.Map(minVerKey);
            var appConfig = await configurationService.LoadAppConfig(retrieveCached);
            var minimumVersionMappedValue = appConfig?.GetValueForKey(minVerKey);
            var appVerFromPlatform = configurationService.VersionService?.GetVersion();

            if (appVerFromPlatform == null || minimumVersionMappedValue == null)
            {
                return false;
            }

            // Check the minimum required version first
            Version versionToCheck;
            if (Version.TryParse(minimumVersionMappedValue, out versionToCheck) &&
                versionToCheck > appVerFromPlatform) //.CompareTo(appVerFromPlatform) > 0
            {
                // Minimum version is lower than app version
                return false;
            }
            return true;
            //if (Version.TryParse(minimumVersionMappedValue, out versionToCheck) &&
            //    versionToCheck.CompareTo(appVersion) > 0)
            //{
            //    // Non-blocking Update avaialable
            //    isValidVersionNumber = true;
            //}

            //return isValidVersionNumber;
        }
    }
}