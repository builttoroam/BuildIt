using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Standard.Models;
using MvvmCross.Platform;
using MvvmCross.Plugins.File;

namespace Client.iOS.Impl
{
    public class iOSFileCacheService : IFileCacheService
    {
        private IMvxFileStore fileService;
        private const string StoreFileName = "Config.xml";
        private Environment.SpecialFolder LocalPath => Environment.SpecialFolder.MyDocuments;
        private IEnumerable<AppConfiguration> _config;
        public IEnumerable<AppConfiguration> Config
        {
            get { return _config; }
        }

        private void MvvmcrossPluginLoaded()
        {
            fileService = Mvx.Resolve<IMvxFileStore>();
        }

        public void Save(AppConfiguration appConfiguration)
        {
            MvvmcrossPluginLoaded();
            fileService.WriteFile(StoreFileName, (stream) =>
            {
                var serializer = new XmlSerializer(typeof(List<AppConfiguration>));
                serializer.Serialize(stream, Config.ToList());
            });
        }

        public async Task<AppConfiguration> LoadConfigData(string relativePath)
        {
            MvvmcrossPluginLoaded();
            throw new System.NotImplementedException();
        }

        public async Task ClearData()
        {
            MvvmcrossPluginLoaded();
            throw new System.NotImplementedException();
        }
    }
}