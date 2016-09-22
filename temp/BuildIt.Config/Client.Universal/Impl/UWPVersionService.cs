using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using BuildIt.Config.Core.Api.Models;
using BuildIt.Config.Core.Services;
using BuildIt.Config.Core.Services.Interfaces;

namespace Client.Universal.Impl
{
    public class UWPVersionService : IVersionService
    {
        public AppConfigurationValue GetVersion()
        {
            string appVersion =
                $"{Package.Current.Id.Version.Major}" +
                $".{Package.Current.Id.Version.Minor}" +
                $".{Package.Current.Id.Version.Build}" +
                $".{Package.Current.Id.Version.Revision}";
            return new AppConfigurationValue
            {
                Value = appVersion
            };
        }
    }
}
