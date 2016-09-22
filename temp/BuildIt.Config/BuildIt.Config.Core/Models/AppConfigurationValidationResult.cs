using System.Collections.Generic;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Models
{
    public class AppConfigurationValidationResult
    {
        public bool IsValid { get; set; }

        public List<AppConfigurationValue> InvalidValues { get; set; } = new List<AppConfigurationValue>();
    }
}
