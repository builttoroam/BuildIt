namespace BuildIt.Config.Core.Standard
{
    public class Constants
    {
        public const string DefaultAppConfigurationControllerName = "appconfiguration";
        public const string DefaultAppConfigurationControllerActionName = "app";
        public const string DefaultAppConfigurationControllerPrefix = "api";
        public const string AppConfigurationNotFoundError = "Something went wrong we couldn't retrieve your app configuration";
        public const string AppConfigurationKeyNotFoundError = "Given key is not present";
        public const string AppConfigurationMinVersionKey = "App_VersionInfo_MinimumAppVersion";
        public const string AppConfigurationMinimumVersionNotMetUserDialogTitle = "App version mismatch";
        public const string AppConfigurationMinimumVersionNotMetUserDialogBody = "Your app version does not meet up the minimum version required!";
        public const string AppConfigurationRecommVersionKey = "App_VersionInfo_RecommendedAppVersion";
        public const string AppConfigurationRecommendedVersionNotMetUserDialogTitle = "More recommended version available";
        public const string AppConfigurationRecommendedVersionNotMetUserDialogBody = "Your app version does not match with the version recommended!";
        public const string AppConfigurationServiceNotificationTitleKey = "App_ServiceNotification_Title";
        public const string AppConfigurationServiceNotificationBodyKey = "App_ServiceNotification_Body";
        public const string AppConfigurationServiceNotificationDisplayingKey = "App_ServiceNotification_Displaying";
    }
}
