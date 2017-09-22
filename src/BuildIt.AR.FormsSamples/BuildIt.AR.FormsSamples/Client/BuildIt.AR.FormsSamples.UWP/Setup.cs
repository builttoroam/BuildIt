using Windows.ApplicationModel.Activation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using Xamarin.Forms;
using BuildIt.AR.FormsSamples.Core;
using BuildIt.AR.FormsSamples.UI;
using MvvmCross.Forms.Core;
using MvvmCross.Forms.Uwp.Presenters;
using MvvmCross.Platform.Platform;
using MvvmCross.Uwp.Platform;
using MvvmCross.Uwp.Views;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using XamlControls = Windows.UI.Xaml.Controls;

namespace BuildIt.AR.FormsSamples.UWP
{
    public class Setup : MvxWindowsSetup
    {
        private readonly LaunchActivatedEventArgs _launchActivatedEventArgs;
        public const string UwpApplicationInsightsAppId = "";
        public const string UwpApplicationInsightsSecretKey = "";

        public Setup(XamlControls.Frame rootFrame, LaunchActivatedEventArgs e) : base(rootFrame)
        {
            _launchActivatedEventArgs = e;
        }

        protected override IMvxApplication CreateApp()
        {
            return new MvvmcrossApp();
        }
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override IMvxWindowsViewPresenter CreateViewPresenter(IMvxWindowsFrame rootFrame)
        {
            Forms.Init(_launchActivatedEventArgs);
            var app = new MvxFormsApplication();
            Mvx.LazyConstructAndRegisterSingleton<IViewLookupInitialiser, FormsViewLookupInitializer>();

            var presenter = new MvxFormsUwpPagePresenter(rootFrame, app);
            Mvx.RegisterSingleton<IMvxViewPresenter>(presenter);
            return presenter;
        }

        protected override void InitializeViewLookup()
        {
            base.InitializeViewLookup();

            var initializer = Mvx.Resolve<IViewLookupInitialiser>();
            initializer?.InitializeViewLookup();
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            var config = Mvx.Resolve<IConfigurationService>().CurrentConfiguration;
            config.ApplicationInsightsAppId = UwpApplicationInsightsAppId;
            config.ApplicationInsightsSecretKey = UwpApplicationInsightsSecretKey;
        }
    }
}
