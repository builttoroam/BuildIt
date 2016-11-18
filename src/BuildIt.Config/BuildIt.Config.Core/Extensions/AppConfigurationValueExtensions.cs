using BuildIt.Config.Core.Models;

namespace BuildIt.Config.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppConfigurationValueExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static T GetValueForKey<T>(this AppConfiguration config, string mappingKey)
        {
            if (config == null || string.IsNullOrEmpty((mappingKey)) || config[mappingKey] == null) return default(T);

            return config[mappingKey].GetValue<T>();
        }
    }
}
