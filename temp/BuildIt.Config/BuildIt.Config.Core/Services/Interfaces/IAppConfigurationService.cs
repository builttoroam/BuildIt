using System.Threading.Tasks;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfigurationMapper Mapper { get; }
        AppConfiguration AppConfig { get; }
        IVersionService VersionService { get; }


        Task<AppConfiguration> LoadAppConfig(bool retrieveCachedVersion = true);

        //Task<bool> CheckMinimumVersion(bool retrieveCachedVersion = false);

        void InitForMvvmCross();
    }
}
