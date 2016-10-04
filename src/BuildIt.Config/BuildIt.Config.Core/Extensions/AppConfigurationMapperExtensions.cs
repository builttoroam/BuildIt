using BuildIt.Config.Core.Models;

namespace BuildIt.Config.Core.Extensions
{
    public static class AppConfigurationMapperExtensions
    {
        public static AppConfigurationMapperAttributes IsRequired(this AppConfigurationMapperAttributes configValue, bool isRequired = true)
        {
            if (configValue == null) return null;

            configValue.ValueIsRequired = isRequired;

            return configValue;
        }

        public static AppConfigurationMapperAttributes IsJson(this AppConfigurationMapperAttributes configValue, bool isJson = true)
        {
            if (configValue == null) return null;

            configValue.ValueIsJson = isJson;

            return configValue;
        }

        public static AppConfigurationMapperAttributes HasFormat(this AppConfigurationMapperAttributes configValue, string format)
        {
            if (configValue == null) return null;

            configValue.Format = format;

            return configValue;
        }
    }
}
