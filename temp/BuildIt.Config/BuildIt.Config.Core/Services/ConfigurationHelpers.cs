using System;
using System.Threading;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Extensions;
using BuildIt.Config.Core.Standard.Services.Interfaces;

namespace BuildIt.Config.Core.Standard.Services
{
    public static class ConfigurationHelpers
    {
        private static SemaphoreSlim SemaphoreSlim { get; } = new SemaphoreSlim(1);

        public static async Task<bool> CheckMinimumVersion(this IAppConfigurationService configurationService, bool handleLoadConfigValidation = false, bool retrieveCached = false, string minVerKey = Constants.AppConfigurationMinVersionKey)
        {
            if (configurationService == null) return false;

            configurationService.Mapper.EnsurePresence(minVerKey, true);
            var appConfig = await configurationService.LoadAppConfig(handleLoadConfigValidation, retrieveCached);
            if (appConfig == null) return false;

            var minimumVersionMappedValue = appConfig.GetValueForKey<string>(minVerKey);
            var appVerFromPlatform = configurationService.VersionService?.GetVersion();

            if (appVerFromPlatform == null || minimumVersionMappedValue == null)
            {
                return false;
            }

            // Check the minimum required version first
            Version versionToCheck;
            return !Version.TryParse(minimumVersionMappedValue, out versionToCheck) || versionToCheck <= appVerFromPlatform;
        }

        public static async Task NotifyUserWhenNotMetAppMinVer(this IAppConfigurationService configurationService, bool handleLoadConfigValidation = false, bool retrieveCached = false, string minVerKey = Constants.AppConfigurationMinVersionKey)
        {
            var metMinimumAppVer = await CheckMinimumVersion(configurationService, handleLoadConfigValidation, retrieveCached, minVerKey);

            if (!metMinimumAppVer)
            {
                //Block the app from running & alert users
                await configurationService.BlockAppFromRunning(Constants.AppConfigurationMinimumVersionNotMetUserDialogTitle, Constants.AppConfigurationMinimumVersionNotMetUserDialogBody, async () =>
                {
                    await NotifyUserWhenNotMetAppMinVer(configurationService, handleLoadConfigValidation, retrieveCached, minVerKey);
                });
            }
        }

        public static async Task<bool> CheckRecommendedVersion(this IAppConfigurationService configurationService, bool retrieveCached = false, string recommVerKey = Constants.AppConfigurationRecommVersionKey)
        {
            if (configurationService == null) return false;

            configurationService.Mapper.EnsurePresence(recommVerKey);

            var appConfig = await configurationService.LoadAppConfig(retrieveCached);
            var recommVersionMappedValue = appConfig?.GetValueForKey<string>(recommVerKey);
            var appVerFromPlatform = configurationService.VersionService?.GetVersion();

            if (appVerFromPlatform == null || recommVersionMappedValue == null)
            {
                return false;
            }

            Version versionToCheck;
            return !Version.TryParse(recommVersionMappedValue, out versionToCheck) || versionToCheck <= appVerFromPlatform;
        }

        public static async Task NotifyUserWhenNotMetAppRecommendedVer(this IAppConfigurationService configurationService, bool retrieveCached = false, Action failureHandler = null, string recommVerKey = Constants.AppConfigurationRecommVersionKey)
        {
            var metRecommendedVer = await CheckRecommendedVersion(configurationService, retrieveCached, recommVerKey);

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
                    await configurationService.BlockAppFromRunning(Constants.AppConfigurationRecommendedVersionNotMetUserDialogTitle, Constants.AppConfigurationRecommendedVersionNotMetUserDialogBody);
                }
            }
        }

        public static async Task HandleServiceNotification(this IAppConfigurationService configurationService, bool retrieveCached = false, Action failureHandler = null,
                                                           string serviceNotificationTitleKey = Constants.AppConfigurationServiceNotificationTitleKey,
                                                           string serviceNotificationBodyKey = Constants.AppConfigurationServiceNotificationBodyKey,
                                                           string serviceNotificationShowKey = Constants.AppConfigurationServiceNotificationDisplayingKey)
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
                        await configurationService.BlockAppFromRunning(serviceNotificationBody, serviceNotificationTitle);
                    }
                }
            }
        }

        public static async Task BlockAppFromRunning(this IAppConfigurationService configurationService, string title, string body, Func<Task> retryAction = null)
        {
            await SemaphoreSlim.WaitAsync();
            try
            {
                await configurationService.UserDialogService.AlertAsync(body, title);
                await Task.Delay(1000);
                retryAction?.Invoke();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}