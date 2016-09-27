using System;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Models;

namespace BuildIt.Config.Core.Standard.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfigurationMapper Mapper { get; }
        AppConfiguration AppConfig { get; }
        IVersionService VersionService { get; }

        IUserDialogService UserDialogService { get; }

        Task<AppConfiguration> LoadAppConfig(bool retrieveCachedVersion = true);
    }
}
