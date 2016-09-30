using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using BuildIt.Config.Core;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Core.Utilities;
using BuildIt.Config.Impl.Common;
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
            Mvx.RegisterSingleton(() => UserDialogs.Instance);
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationEndpointService>(() => new AppConfigurationEndpointService(new AppConfigurationRoutingModel()
            {
                BaseUrl = "http://fnmservices-dev.azurewebsites.net",
                Prefix = "test1",
                Controller = "configuration"
            }));
            Mvx.LazyConstructAndRegisterSingleton<INetworkService, NetworkService>();
            Mvx.LazyConstructAndRegisterSingleton<IUserDialogService, UserDialogService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationServiceSetup>(() => new AppConfigurationServiceSetup()
            {
                CacheExpirationTime = new TimeSpan(0, 0, 10)
            });
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationService, AppConfigurationService>();
            Mvx.LazyConstructAndRegisterSingleton<IAppConfigurationRequiredServices, AppConfigurationRequiredServices>();

            InitAppConfig();

            Mvx.RegisterSingleton<IMvxAppStart>(new MvxAppStart<MainViewModel>());
        }

        private async void InitAppConfig()
        {
            var appConfigService = Mvx.Resolve<IAppConfigurationService>();
            //appConfigService?.InitForMvvmCross();
            // Step 1: Retrieve App config from Azure
            if (appConfigService == null) return;

            appConfigService.AdditionalHeaders.Add(new KeyValuePair<string, string>(Strings.ApiKey, Constants.AppConfigurationApiKey));
            appConfigService.Mapper.EnsurePresence("App_VersionInfo_CurrentAppVersion", true);

            //await appConfigService.LoadAppConfig();

            // Step 2: Check the minimum version & block the app from running if it's not met
            await appConfigService.NotifyUserWhenNotMetAppMinVer();
            // Step 3: Check the recommended version & alert users if it's not met
            //await appConfigService.CheckRecommendedVersion();
        }
    }
}
