#if NET45

using System;
using System.Configuration;

namespace BuildIt.Web.Utitlites
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="settingsKey"></param>
        /// <returns></returns>
        public static string EnvironmentValue(this string settingsKey)
        {
            var value = Environment.GetEnvironmentVariable(settingsKey);

            if (!string.IsNullOrWhiteSpace(value)) return value;
            var environmentValue = ConfigurationManager.AppSettings[settingsKey];
            return environmentValue;
        }
    }
}

#endif