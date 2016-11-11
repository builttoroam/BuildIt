using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Web.Utilities;

namespace BuildIt.Web.Models.Routing
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomControllerRoutingModel<T>
    {
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
        public static CustomControllerRoutingModel<T> Default => new CustomControllerRoutingModel<T>()
        {
            Version = null,
            Prefix = Constants.DefaultAppConfigurationControllerPrefix,
            Controller = nameof(T).Replace("Controller", "")
        };
    }
}