using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface IAppConfigurationService
    {
        AppConfigurationMapper Mapper { get; }
        AppConfiguration AppConfig { get; }
        IVersionService VersionService { get; }
        Task BlockAppFromRunning<T>(string title, string body, Func<T> callerFunc);
        Task<AppConfiguration> LoadAppConfig(bool retrieveCachedVersion = true);

        //Task<bool> CheckMinimumVersion(bool retrieveCachedVersion = false);

        void InitForMvvmCross();
    }
}
