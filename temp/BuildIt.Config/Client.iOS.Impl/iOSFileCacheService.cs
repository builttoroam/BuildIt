using System.Threading.Tasks;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Standard.Models;

namespace Client.iOS.Impl
{
    public class iOSFileCacheService : IFileCacheService
    {
        public async Task<bool> SaveConfigData(AppConfiguration configValues)
        {
            throw new System.NotImplementedException();
        }

        public async Task<AppConfiguration> LoadConfigData(string relativePath)
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> SaveToPath<TData>(string path, TData data) where TData : class
        {
            throw new System.NotImplementedException();
        }

        public async Task<TData> LoadFromPath<TData>(string path) where TData : class
        {
            throw new System.NotImplementedException();
        }

        public async Task ClearData()
        {
            throw new System.NotImplementedException();
        }
    }
}