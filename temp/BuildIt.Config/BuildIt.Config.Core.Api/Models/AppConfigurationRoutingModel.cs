using BuildIt.Config.Core.Api.Utilities;

namespace BuildIt.Config.Core.Api.Models
{
    public class AppConfigurationRoutingModel
    {
        public string Version { get; set; }
        public string Prefix { get; set; }

        public string Controller { get; set; }
        public string Action { get; set; }

        public static AppConfigurationRoutingModel Default => new AppConfigurationRoutingModel()
        {
            Version = null,
            Prefix = Constants.DefaultAppConfigurationControllerPrefix,
            Controller = Constants.DefaultAppConfigurationControllerName,
            Action = Constants.DefaultAppConfigurationControllerActionName
        };


    }
}
