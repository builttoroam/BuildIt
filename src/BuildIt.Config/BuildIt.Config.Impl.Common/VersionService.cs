using System;
using BuildIt.Config.Core.Services.Interfaces;
using Plugin.VersionTracking;

namespace BuildIt.Config.Impl.Common
{
    public class VersionService : IVersionService
    {
        public Version GetVersion()
        {
            return new Version(CrossVersionTracking.Current.CurrentVersion);
        }
    }
}
