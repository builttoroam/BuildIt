using System;
using System.Threading.Tasks;

namespace BuildIt.Config.Core.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationMapperAttributes
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool ValueIsRequired { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ValueIsBlocking { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ValueIsJson { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Func<AppConfigurationValue, Task> FailureHandler { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AppConfigurationMapperAttributes()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public AppConfigurationMapperAttributes(string name)
        {
            Name = name;
        }
    }
}
