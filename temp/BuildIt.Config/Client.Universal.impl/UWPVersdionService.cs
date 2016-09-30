using System;
using Windows.ApplicationModel;
using BuildIt.Config.Core.Services.Interfaces;

namespace Client.Universal.Impl
{
    public class UWPVersionService : IVersionService
    {
        public Version GetVersion()
        {
            return new Version(Package.Current.Id.Version.Major,
                Package.Current.Id.Version.Minor,
                Package.Current.Id.Version.Build,
                Package.Current.Id.Version.Revision);
        }
    }
}
