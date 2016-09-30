using BuildIt.Config.Core.Standard.Models;

namespace BuildIt.Config.Core.Standard.Extensions
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
