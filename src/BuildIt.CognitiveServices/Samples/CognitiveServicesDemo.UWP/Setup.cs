using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Forms.Core;
using MvvmCross.Forms.Uwp.Presenters;
using MvvmCross.Platform;
using MvvmCross.Uwp.Platform;
using MvvmCross.Uwp.Views;
using Windows.ApplicationModel.Activation;
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

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            Forms.Init(launchActivatedEventArgs);
            var app = new MvxFormsApplication();

            var presenter = new MvxFormsUwpPagePresenter(rootFrame, app);
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);

            return presenter;
        }
    }
}