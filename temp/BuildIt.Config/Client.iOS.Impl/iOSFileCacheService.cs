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
        private IMvxFileStore FileService { get; } = Mvx.Resolve<IMvxFileStore>();
        private const string StoreFileName = "Config.xml";
        private Environment.SpecialFolder LocalPath => Environment.SpecialFolder.MyDocuments;
        private IEnumerable<AppConfiguration> _config;
        public IEnumerable<AppConfiguration> Config
        {
            get { return _config; }
        }

        public void Save(AppConfiguration appConfiguration)
        {
            FileService.WriteFile(StoreFileName, (stream) =>
            {
                var serializer = new XmlSerializer(typeof(List<AppConfiguration>));
                serializer.Serialize(stream, Config.ToList());
            });
        }

        public async Task<AppConfiguration> LoadConfigData(string relativePath)
        {
            throw new System.NotImplementedException();
        }

        public async Task ClearData()
        {
            throw new System.NotImplementedException();
        }
    }
}