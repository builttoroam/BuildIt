using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfiguration
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Keys => valuesDict.Keys.ToList();
        /// <summary>
        /// 
        /// </summary>
        public List<AppConfigurationValue> Values => valuesDict.Values.ToList();

        private readonly Dictionary<string, AppConfigurationValue> valuesDict;

        /// <summary>
        /// 
        /// </summary>
        public AppConfiguration()
        {
            this.valuesDict = new Dictionary<string, AppConfigurationValue>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AppConfigurationValue this[string key]
        {
            get
            {
                if (valuesDict.ContainsKey(key))
                {
                    return valuesDict[key];
                }

                return null;
            }
            set
            {
                if (!valuesDict.ContainsKey(key))
                {
                    valuesDict.Add(key, value);
                }
                else
                {
                    valuesDict[key] = value;
                }
            }
        }
    }
}
