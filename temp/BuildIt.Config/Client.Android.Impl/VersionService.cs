using System;
using Android.Content;
using BuildIt.Config.Core.Standard.Services.Interfaces;

namespace Client.Android.Impl
{
    public class VersionService : IVersionService
    {
        public Context Context { get; set; }

        /// <summary>
        /// Ensure that Context is set before calling GetVersion()
        /// e.g. versionService.Context = ApplicationContext; (called from MainActivity)
        /// </summary>
        /// <returns>null if Context not set</returns>
        public Version GetVersion()
        {
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