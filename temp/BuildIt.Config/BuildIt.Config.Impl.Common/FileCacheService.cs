using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Impl.Common
{
    public class FileCacheService : IFileCacheService
    {
        public Task<bool> Save(AppConfiguration appConfiguration)
        {
            throw new NotImplementedException();
        }

        public Task<AppConfiguration> LoadConfigData()
        {
            throw new NotImplementedException();
        }

        public Task ClearData()
        {
            throw new NotImplementedException();
        }
    }
}
