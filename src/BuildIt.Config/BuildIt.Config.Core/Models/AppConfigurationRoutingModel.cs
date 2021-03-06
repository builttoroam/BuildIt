﻿namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationRoutingModel
    {
        /// <summary>
        /// Use only on the client side to define your server base url
        /// NOTE: Don't include backslash '/' at the end of your url
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static AppConfigurationRoutingModel Default => new AppConfigurationRoutingModel()
        {
            Version = null,
            Prefix = Constants.DefaultAppConfigurationControllerPrefix,
            Controller = Constants.DefaultAppConfigurationControllerName
        };
    }
}
