using System.Collections.Generic;
using System.Linq;

namespace BuildIt.Config.Core.Models
{
    public class AppConfiguration
    {
        public List<string> Keys => valuesDict.Keys.ToList();
        public List<AppConfigurationValue> Values => valuesDict.Values.ToList();

        private readonly Dictionary<string, AppConfigurationValue> valuesDict;

        public AppConfiguration()
        {
            this.valuesDict = new Dictionary<string, AppConfigurationValue>();
        }

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
