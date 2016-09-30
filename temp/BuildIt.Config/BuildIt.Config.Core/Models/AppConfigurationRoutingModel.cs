namespace BuildIt.Config.Core.Models
{
    public class AppConfigurationRoutingModel
    {
        /// <summary>
        /// Use only on the client side to define your server base url
        /// NOTE: Don't include backslash '/' at the end of your url
        /// </summary>
        public string BaseUrl { get; set; }

        public string Version { get; set; }
        public string Prefix { get; set; }

        public string Controller { get; set; }

        public static AppConfigurationRoutingModel Default => new AppConfigurationRoutingModel()
        {
            Version = null,
            Prefix = Constants.DefaultAppConfigurationControllerPrefix,
            Controller = Constants.DefaultAppConfigurationControllerName
        };
    }
}
