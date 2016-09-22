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

        private void InitAppConfig()
        {
            var appConfigService = Mvx.Resolve<IAppConfigurationService>();
            appConfigService?.Mapper.EnsurePresence("App_VersionInfo_CurrentAppVersion", true);

            appConfigService?.InitForMvvmCross();
        }
    }
}
