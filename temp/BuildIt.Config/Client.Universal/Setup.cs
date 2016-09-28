using Windows.UI.Xaml.Controls;
using BuildIt.Config.Core.Standard.Services.Interfaces;
using Client.Universal.impl;
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
            Mvx.RegisterType<IUserDialogService, UserDialogService>();
            Mvx.RegisterType<IVersionService, UWPVersionService>();
        }

        protected override IMvxApplication CreateApp()
        {

            return new Client.Core.App();
        }
    }
}
