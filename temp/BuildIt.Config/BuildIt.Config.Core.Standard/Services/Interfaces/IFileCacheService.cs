using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IFileCacheService
    {
        Task<bool> SaveConfigData(AppConfiguration configValues);

        Task<AppConfiguration> LoadConfigData(string relativePath);

        Task<bool> SaveToPath<TData>(string path, TData data) where TData : class;

        Task<TData> LoadFromPath<TData>(string path) where TData : class;

        Task ClearData();
    }
}
