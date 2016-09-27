using BuildIt.Config.Core.Standard.Services;
using BuildIt.Config.Core.Standard.Services.Interfaces;
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
            //appConfigService?.InitForMvvmCross();
            // Step 1: Retrieve App config from Azure
            if (appConfigService == null) return;

            appConfigService.Mapper.EnsurePresence("App_VersionInfo_CurrentAppVersion", true);

            await appConfigService.LoadAppConfig();

            // Step 2: Check the minimum version & block the app from running if it's not met
            await appConfigService.CheckMinimumVersion();
            // Step 3: Check the recommended version & alert users if it's not met
            //await appConfigService.CheckRecommendedVersion();
        }
    }
}
