namespace BuildIt.Config.Core.Standard.Models
{
    public class AppConfigurationRoutingModel
    {
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
