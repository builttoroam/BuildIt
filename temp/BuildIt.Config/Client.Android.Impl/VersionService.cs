using System;
using System.Diagnostics;
using Android.Content;
using BuildIt.Config.Core.Services.Interfaces;

namespace Client.Android.Impl
{
    public class VersionService : IVersionService
    {
        public Context Context { get; set; }

        /// <summary>
        /// Ensure that Context is set before calling this method
        /// e.g. versionService.Context = this; (where 'this' is your MainActivity)
        /// </summary>
        /// <returns>null if Context not set</returns>
        public Version GetVersion()
        {
            if (Context == null)
            {
                Debug.WriteLine("You must assign Context before calling this method! (e.g. Context = this, where 'this' is MainActivity)");
                return null;
            }

            var version = Context?.PackageManager?.GetPackageInfo(Context?.PackageName, 0).VersionName;
            if (string.IsNullOrWhiteSpace(version)) return null;

            return new Version(version);
        }

        public VersionService()
        {

        }
        public VersionService(Context context)
        {
            Context = context;
        }
    }
}