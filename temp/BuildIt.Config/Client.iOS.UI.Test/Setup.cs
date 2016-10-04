using BuildIt.Config.Core.Services.Interfaces;
using BuildIt.Config.Impl.iOS;
using Client.Core;
using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;

namespace Client.iOS.UI.Test
{
    public class Setup : MvxIosSetup
    {
        public Setup(MvxApplicationDelegate appDelegate, IMvxIosViewPresenter presenter)
            : base(appDelegate, presenter)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new App();
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            //Mvx.LazyConstructAndRegisterSingleton<IUserDialogService, UserDialogService>();
            Mvx.LazyConstructAndRegisterSingleton<IVersionService, iOSVersionService>();
            //Mvx.LazyConstructAndRegisterSingleton<IFileCacheService, FileCacheService>();
        }
    }
}