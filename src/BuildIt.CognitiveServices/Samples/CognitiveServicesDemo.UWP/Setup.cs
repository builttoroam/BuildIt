using Windows.ApplicationModel.Activation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Presenter.Core;
using MvvmCross.Forms.Presenter.WindowsUWP;
using MvvmCross.Platform;
using MvvmCross.WindowsUWP.Platform;
using MvvmCross.WindowsUWP.Views;
using Xamarin.Forms;
using XamlControls = Windows.UI.Xaml.Controls;

namespace CognitiveServicesDemo.UWP
{
    public class Setup : MvxWindowsSetup
    {
        private readonly LaunchActivatedEventArgs launchActivatedEventArgs;

        public Setup(XamlControls.Frame rootFrame, LaunchActivatedEventArgs e) : base(rootFrame)
        {
            launchActivatedEventArgs = e;
        }

        protected override IMvxApplication CreateApp()
        {
            var app = new CognitiveServicesDemo.App();

            return app;
        }

        protected override void InitializeIoC()
        {
            base.InitializeIoC();
            //Mvx.LazyConstructAndRegisterSingleton<IMeetingsService, MeetingsService>();
            //Mvx.LazyConstructAndRegisterSingleton<IPhotoPropertiesService, PhotoPropertiesService>();
        }

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            Forms.Init(launchActivatedEventArgs);
            var app = new MvxFormsApp();

            var presenter = new MvxFormsWindowsUWPPagePresenter(rootFrame, app);
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);

            return presenter;
        }
    }
}
