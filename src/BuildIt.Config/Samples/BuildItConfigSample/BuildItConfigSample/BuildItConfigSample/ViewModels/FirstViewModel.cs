using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildIt.Config.Core.Extensions;
using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services;
using BuildIt.Config.Core.Services.Interfaces;
using MvvmCross.Core.ViewModels;

namespace BuildItConfigSample.ViewModels
{
    public class FirstViewModel : MvxViewModel
    {
        private readonly IAppConfigurationService appConfigurationService;

        private ICommand getAppConfigCommand;
        public ICommand GetAppConfigCommand => getAppConfigCommand ?? (getAppConfigCommand = new MvxCommand(async () => await GetAppConfig()));


        public ObservableCollection<AppConfigurationValue> ConfigValues { get; } = new ObservableCollection<AppConfigurationValue>();
        public FirstViewModel(IAppConfigurationService appConfigurationService)
        {
            this.appConfigurationService = appConfigurationService;
        }

        public override async void Start()
        {
            base.Start();

            MapConfig();
            await GetAppConfig();
        }

        private void MapConfig()
        {
            appConfigurationService.Mapper.Map("App_VersionInfo_MinimumAppVersion")
                                          .IsRequired();

            appConfigurationService.Mapper.Map("App_VersionInfo_CurrentAppVersion")
                                          .IsRequired();

            appConfigurationService.Mapper.Map("App_States")
                                          .IsJson();
        }

        public async Task GetAppConfig()
        {
            ConfigValues.Clear();

            var config = await appConfigurationService.LoadAppConfig(false);
            if (config == null) return;

            foreach (var value in config.Values)
            {
                ConfigValues.Add(value);
            }

            //await appConfigurationService.CheckMinimumVersion();
        }
    }
}
