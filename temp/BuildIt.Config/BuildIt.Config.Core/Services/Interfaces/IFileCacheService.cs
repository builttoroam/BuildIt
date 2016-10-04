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
        bool HasExpired { get; }
        event EventHandler CacheExpired;

        Task<bool> Save(AppConfiguration appConfiguration, TimeSpan expirationTime = default(TimeSpan));

        Task<AppConfiguration> LoadConfigData();

        Task ClearData();
    }
}
