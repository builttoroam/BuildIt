namespace BuildIt.Config.Core.Utilities
{
    public enum AppConfigurationMode
    {
        /// <summary>
        /// App needs to retrieve configuration every time launched
        /// </summary>
        OnlyOnline,
        /// <summary>
        /// App needs to retrieve configuration at least ONCE
        /// and then it can work with cached version of config
        /// </summary>
        AllowOffline
    }
}
