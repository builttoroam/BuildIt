using System.Collections.Generic;

namespace BuildIt.Config.Core.Models
{
    public class AppConfigurationValidationResult
    {
        public bool IsValid { get; set; }

        public List<AppConfigurationValue> InvalidValues { get; set; } = new List<AppConfigurationValue>();
    }
}
