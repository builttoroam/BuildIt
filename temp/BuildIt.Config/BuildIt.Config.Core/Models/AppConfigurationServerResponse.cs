using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Config.Core.Standard.Models
{
    public class AppConfigurationServerResponse
    {
        public bool HasErrors => AppConfigErors?.Any() ?? false;
        public bool HasConfigValues => AppConfigValues?.Any() ?? false;

        public List<AppConfigurationValue> AppConfigValues { get; set; } = new List<AppConfigurationValue>();
        public List<AppConfigurationError> AppConfigErors { get; set; }
    }
}
