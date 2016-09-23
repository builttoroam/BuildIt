using BuildIt.Config.Core.Services;
using BuildIt.Config.Core.Services.Interfaces;
using Client.Core.Services;
using Client.Core.ViewModels;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Client.Core
{
    public class App : MvxApplication
    {
        public App()
        {
            Mvx.ConstructAndRegisterSingleton<IAppConfigurationServiceEndpoint, AppConfigurationServiceEndpoint>();
            Mvx.ConstructAndRegisterSingleton<IAppConfigurationService, AppConfigurationService>();

            InitAppConfig();

            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<MainViewModel>());
        }

        private async void InitAppConfig()
        {
            var appConfigService = Mvx.Resolve<IAppConfigurationService>();
            appConfigService?.InitForMvvmCross();
            // Step 1: Retrieve App config from Azure
            if (appConfigService == null) return;

            appConfigService?.Mapper.EnsurePresence("App_VersionInfo_CurrentAppVersion", true);
            appConfigService?.Mapper.EnsurePresence("App_VersionInfo_MinimumRequiredVersion", true);
            await appConfigService.LoadAppConfig();

            // Step 2: Check the minimum versioin
            await appConfigService.CheckMinimumVersion();
        }
    }
}
