using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BuildIt.Config.Core.Api.Models;
using BuildIt.Config.Core.Extensions;
using BuildIt.Config.Core.Services.Interfaces;
using MvvmCross.Platform;

namespace BuildIt.Config.Core.Services
{
    public static class ConfigurationHelpers
    {
        public static async Task<bool> CheckMinimumVersion(this IAppConfigurationService configurationService, bool retrieveCached = false, string minVerKey = "App_VersionInfo_MinimumRequiredVersion")
        {
            if (configurationService == null) return false;

            configurationService?.Mapper.EnsurePresence(minVerKey, true);

            var metMinimumAppVer = true;
            //Version appVersion;
            //Mapper.Map(minVerKey);
            var appConfig = await configurationService.LoadAppConfig(retrieveCached);
            if (appConfig == null) return false;

            var minimumVersionMappedValue = appConfig.GetValueForKey<string>(minVerKey);
            var appVerFromPlatform = configurationService.VersionService?.GetVersion();

            if (appVerFromPlatform == null || minimumVersionMappedValue == null)
            {
                //return false;
                metMinimumAppVer = false;
            }

            // Check the minimum required version first
            Version versionToCheck;
            if (Version.TryParse(minimumVersionMappedValue, out versionToCheck) &&
                versionToCheck > appVerFromPlatform) //.CompareTo(appVerFromPlatform) > 0
            {
                // App version is lower than minimum version
                //return false;
                metMinimumAppVer = false;
            }

            if (!metMinimumAppVer)
            {
                //Block the app from running & alert users
                await configurationService.BlockAppFromRunning("App version mismatch", "Your app version does not meet up the minimum version required!", async () =>
                {
                    await CheckMinimumVersion(configurationService, retrieveCached, minVerKey);
                });
            }

            return metMinimumAppVer;
            //return true;
            //if (Version.TryParse(minimumVersionMappedValue, out versionToCheck) &&
            //    versionToCheck.CompareTo(appVersion) > 0)
            //{
            //    // Non-blocking Update avaialable
            //    isValidVersionNumber = true;
            //}

            //return isValidVersionNumber;
        }

        public static async Task<bool> CheckRecommendedVersion(this IAppConfigurationService configurationService, bool retrieveCached = false, Action failureHandler = null, string recommVerKey = "App_VersionInfo_RecommendedVersion")
        {
            if (configurationService == null) return false;

            var metRecommendedVer = true;
            configurationService.Mapper.EnsurePresence(recommVerKey);

            var appConfig = await configurationService.LoadAppConfig(retrieveCached);
            var recommVersionMappedValue = appConfig?.GetValueForKey<string>(recommVerKey);
            var appVerFromPlatform = configurationService.VersionService?.GetVersion();

            if (appVerFromPlatform == null || recommVersionMappedValue == null)
            {
                //return false;
                metRecommendedVer = false;
            }

            Version versionToCheck;
            if (Version.TryParse(recommVersionMappedValue, out versionToCheck) &&
                versionToCheck > appVerFromPlatform)
            {
                // App version is lower than recommended version
                //return false;
                metRecommendedVer = false;
            }

            if (!metRecommendedVer)
            {
                if (failureHandler != null)
                {
                    //developer might have the custom action rather than displaying out alert
                    failureHandler.Invoke();
                }
                else
                {
                    //Alert users
                    var dialog = Mvx.Resolve<IUserDialogs>();
                    var alertAsync = dialog?.AlertAsync("Your app version does not match with the version recommended!");
                    if (alertAsync != null) await alertAsync;
                }
            }

            //return true;
            return metRecommendedVer;
        }

        public static async Task HandleServiceNotification(this IAppConfigurationService configurationService,
            bool retrieveCached = false, Action failureHandler = null, 
            string serviceNotificationTitleKey = "App_ServiceNotification_Title",
            string serviceNotificationBodyKey = "App_ServiceNotification_Body", 
            string serviceNotificationShowKey = "App_ServiceNotification_Displaying")
        {
            if (configurationService == null) return;

            configurationService.Mapper.EnsurePresence(serviceNotificationTitleKey);
            configurationService.Mapper.EnsurePresence(serviceNotificationBodyKey);
            configurationService.Mapper.EnsurePresence(serviceNotificationShowKey);

            var appConfig = await configurationService.LoadAppConfig(retrieveCached);

            if (appConfig == null) return;

            var showServiceNotification = appConfig.GetValueForKey<bool>(serviceNotificationShowKey);
            if (showServiceNotification)
            {
                //Alert users
                var serviceNotificationTitle = appConfig.GetValueForKey<string>(serviceNotificationTitleKey);
                var serviceNotificationBody = appConfig.GetValueForKey<string>(serviceNotificationBodyKey);
                if (!string.IsNullOrEmpty(serviceNotificationTitle) &&
                    !string.IsNullOrEmpty(serviceNotificationBody))
                {

                    if (failureHandler != null)
                    {
                        //developer might have the custom action rather than displaying out alert
                        failureHandler.Invoke();
                    }
                    else
                    {
                        var dialog = Mvx.Resolve<IUserDialogs>();
                        var alertAsync = dialog?.AlertAsync(serviceNotificationBody, serviceNotificationTitle);
                        if (alertAsync != null) await alertAsync;
                    }
                }
            }
        }
    }
}