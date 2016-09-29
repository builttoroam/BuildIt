using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Services;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;

namespace Client.Android.Test
{
    public class MainViewModel : MvxViewModel
    {
        public MainViewModel()
        {

        }

        public override async void Start()
        {
            try
            {
                base.Start();

                await RetrieveConfig();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public async Task RetrieveConfig()
        {
            var appConfigService = Mvx.Resolve<IAppConfigurationService>();
            //appConfigService?.InitForMvvmCross();
            // Step 1: Retrieve App config from Azure
            if (appConfigService == null) return;

            appConfigService.Mapper.EnsurePresence("App_VersionInfo_CurrentAppVersion", true);

            //await appConfigService.LoadAppConfig();

            // Step 2: Check the minimum version & block the app from running if it's not met
            //await appConfigService.NotifyUserWhenNotMetAppMinVer();
            // Step 3: Check the recommended version & alert users if it's not met
            await appConfigService.CheckMinimumVersion();
        }
    }
}