using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IVersionService
    {
        AppConfigurationValue GetVersion();
    }
}
