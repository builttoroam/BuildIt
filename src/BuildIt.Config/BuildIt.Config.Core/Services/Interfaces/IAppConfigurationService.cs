﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BuildIt.Config.Core.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfigurationMapper Mapper { get; }
        AppConfiguration AppConfig { get; }
        IVersionService VersionService { get; }

        IUserDialogService UserDialogService { get; }

        Task<AppConfiguration> LoadAppConfig(bool handleLoadValidation = true, bool retrieveCachedVersion = true);

        /// <summary>
        /// Key: Header name
        /// Value: Header value
        /// </summary>
        List<KeyValuePair<string, string>> AdditionalHeaders { get; set; }
    }
}
