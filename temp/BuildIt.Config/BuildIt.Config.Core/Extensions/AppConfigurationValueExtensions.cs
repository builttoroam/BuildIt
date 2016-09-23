using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Extensions
{
    public static class AppConfigurationValueExtensions
    {
        public static string GetValueForKey(this AppConfiguration config, string mappingKey)
        {
            if (config == null || string.IsNullOrEmpty((mappingKey))) return null;

            return config[mappingKey]?.Value;
        }
    }
}
