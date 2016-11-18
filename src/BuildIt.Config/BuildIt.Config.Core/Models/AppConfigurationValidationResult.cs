using System.Collections.Generic;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationValidationResult
    {
        /// <summary>
        /// 
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AppConfigurationValue> InvalidValues { get; set; } = new List<AppConfigurationValue>();
    }
}
