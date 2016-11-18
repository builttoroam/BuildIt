using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationMapper
    {
        private readonly Dictionary<string, AppConfigurationMapperAttributes> configMap = new Dictionary<string, AppConfigurationMapperAttributes>();

        /// <summary>
        /// 
        /// </summary>
        public List<AppConfigurationMapperAttributes> MappedValues => new List<AppConfigurationMapperAttributes>(configMap.Values);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappedConfigKey"></param>
        /// <returns></returns>
        public AppConfigurationMapperAttributes Map(string mappedConfigKey)
        {
            if (string.IsNullOrEmpty(mappedConfigKey)) return null;
            if (configMap.ContainsKey(mappedConfigKey)) return configMap[mappedConfigKey];

            var configValue = new AppConfigurationMapperAttributes(mappedConfigKey);
            configMap.Add(mappedConfigKey, configValue);

            return configValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mappedConfigKey"></param>
        /// <param name="isBlocking"></param>
        /// <param name="failureHandler"></param>
        /// <returns></returns>
        public AppConfigurationMapper EnsurePresence(string mappedConfigKey, bool isBlocking = false, Func<AppConfigurationValue, Task> failureHandler = null)
        {
            //TODO: not sure if we should return 'null' if failure or just make it safe to use no matter what and return 'this'
            if (string.IsNullOrEmpty(mappedConfigKey)) return this;

            var mappedConfigValue = this.Map(mappedConfigKey);
            if (mappedConfigValue == null) return this;

            mappedConfigValue.ValueIsRequired = true;
            mappedConfigValue.ValueIsBlocking = isBlocking;
            mappedConfigValue.FailureHandler = failureHandler;

            return this;
        }
    }
}
