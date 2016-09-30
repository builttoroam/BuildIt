using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.Config.Core.Standard.Models
{
    public class AppConfigurationError
    {
        public string Content { get; set; }

        public AppConfigurationMapperAttributes Mapping { get; set; }
    }
}
