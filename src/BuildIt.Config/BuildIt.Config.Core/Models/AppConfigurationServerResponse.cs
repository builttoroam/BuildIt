using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationServerResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public bool HasErrors => AppConfigErors?.Any() ?? false;
        /// <summary>
        /// 
        /// </summary>
        public bool HasConfigValues => AppConfigValues?.Any() ?? false;

        /// <summary>
        /// 
        /// </summary>
        public List<AppConfigurationValue> AppConfigValues { get; set; } = new List<AppConfigurationValue>();
        /// <summary>
        /// 
        /// </summary>
        public List<AppConfigurationError> AppConfigErors { get; set; }
    }
}
