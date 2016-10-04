using Windows.UI.Xaml.Controls;
using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Impl.Common;
using BuildIt.Config.Impl.Win;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.WindowsUWP.Platform;

namespace Client.Universal
{
    public class Setup : MvxWindowsSetup
    {
        public Setup(Frame rootFrame) : base(rootFrame)
        {
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            //Mvx.RegisterType<IUserDialogService, UserDialogService>();
            Mvx.RegisterType<IVersionService, VersionService>();
        }

        protected override IMvxApplication CreateApp()
        {
            return new Client.Core.App();
        }
    }
}
