using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Config.Core.Standard.Models
{
    public class AppConfigurationResponse
    {
        public List<AppConfigurationValue> AppConfigValues { get; set; } = new List<AppConfigurationValue>();
        public List<AppConfigurationError> AppConfigErors { get; set; } = new List<AppConfigurationError>();

        public bool HasErrors() => AppConfigErors != null && AppConfigErors.Any();

        public bool HasConfigValues() => AppConfigValues != null && AppConfigValues.Any();
    }
}
