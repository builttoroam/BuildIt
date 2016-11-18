using System;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationValue
    {
        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppConfigurationMapperAttributes Attributes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public T GetValue<T>()
        {
            if (Attributes == null) return default(T);

            try
            {
                var valueType = typeof(T);

                if (valueType == typeof(DateTime))
                {
                    var date = default(DateTime);
                    if (string.IsNullOrEmpty(Attributes.Format))
                    {
                        DateTime.TryParse(Value, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                    }
                    else
                    {
                        DateTime.TryParseExact(Value, Attributes.Format, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                    }

                    return (T)(object)date;
                }
                else if (valueType == typeof(bool))
                {
                    bool flag;
                    if (bool.TryParse(Value, out flag))
                    {
                        return (T)(object)flag;
                    }
                }
                else if (Attributes.ValueIsJson)
                {
                    return JsonConvert.DeserializeObject<T>(Value);
                }
                else
                {
                    return (T)(object)Value;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            return default(T);
        }
    }
}
