using BuildIt.Config.Core.Models;

namespace BuildIt.Config.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class AppConfigurationMapperExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="isRequired"></param>
        /// <returns></returns>
        public static AppConfigurationMapperAttributes IsRequired(this AppConfigurationMapperAttributes configValue, bool isRequired = true)
        {
            if (configValue == null) return null;

            configValue.ValueIsRequired = isRequired;

            return configValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="isJson"></param>
        /// <returns></returns>
        public static AppConfigurationMapperAttributes IsJson(this AppConfigurationMapperAttributes configValue, bool isJson = true)
        {
            if (configValue == null) return null;

            configValue.ValueIsJson = isJson;

            return configValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configValue"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static AppConfigurationMapperAttributes HasFormat(this AppConfigurationMapperAttributes configValue, string format)
        {
            if (configValue == null) return null;

            configValue.Format = format;

            return configValue;
        }
    }
}
