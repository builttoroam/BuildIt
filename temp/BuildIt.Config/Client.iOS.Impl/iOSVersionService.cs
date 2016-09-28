using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Foundation;

namespace Client.iOS.Impl
{
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
