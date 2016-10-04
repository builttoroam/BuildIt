using System;
using BuildIt.Config.Core.Services.Interfaces;
using Foundation;

namespace BuildIt.Config.Impl.iOS
{
    // ReSharper disable once InconsistentNaming
    public class iOSVersionService : IVersionService
    {
        public Version GetVersion()
        {
            var ver = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"];
            var versionString = ver?.ToString() ?? "";
            return new Version(versionString);
        }
    }
}
