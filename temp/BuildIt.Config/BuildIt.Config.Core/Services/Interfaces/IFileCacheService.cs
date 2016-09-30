using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IFileCacheService
    {
        void Save(AppConfiguration appConfiguration);

        Task<AppConfiguration> LoadConfigData(string relativePath);

        Task ClearData();
    }
}
