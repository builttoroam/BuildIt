using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Extensions
{
    public static class AppConfigurationValueExtensions
    {
        public static T GetValueForKey<T>(this AppConfiguration config, string mappingKey)
        {
            if (config == null || string.IsNullOrEmpty((mappingKey)) || config[mappingKey] == null) return default(T);

            return config[mappingKey].GetValue<T>();
        }
    }
}
