using System.Threading.Tasks;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfigurationMapper Mapper { get; }
        AppConfiguration AppConfig { get; }

        Task<AppConfiguration> RetrieveAppConfig(bool retrieveCachedVersion = true);

        void InitForMvvmCross();
    }
}
