using System;
using System.Threading.Tasks;

namespace BuildIt.Config.Core.Standard.Models
{
    public class AppConfigurationMapperAttributes
    {
        public string Name { get; set; }

        public string Format { get; set; }

        public bool ValueIsRequired { get; set; }
        public bool ValueIsBlocking { get; set; }
        public bool ValueIsJson { get; set; }

        public Func<AppConfigurationValue, Task> FailureHandler { get; set; }

        public AppConfigurationMapperAttributes()
        {

        }
        public AppConfigurationMapperAttributes(string name)
        {
            Name = name;
        }
    }
}
