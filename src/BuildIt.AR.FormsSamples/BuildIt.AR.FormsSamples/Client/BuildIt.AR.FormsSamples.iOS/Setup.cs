using MvvmCross.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform;
using BuildIt.AR.FormsSamples.Core;
using BuildIt.AR.FormsSamples.Core.Services;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using BuildIt.AR.FormsSamples.UI;
using MvvmCross.Forms.Core;
using MvvmCross.Forms.iOS.Presenters;
using MvvmCross.Platform.Platform;
using UIKit;
using Xamarin.Forms;

namespace BuildIt.AR.FormsSamples.iOS
{
    public class Setup : MvxIosSetup
    {
        public const string IosApplicationInsightsAppId = "";
        public const string IosApplicationInsightsSecretKey = "";

        public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
        {
        }

        protected override IMvxApplication CreateApp()
        {
            return new MvvmcrossApp();
        }

        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }

        protected override void InitializeViewLookup()
        {
            base.InitializeViewLookup();

            var initializer = Mvx.Resolve<IViewLookupInitialiser>();
            initializer?.InitializeViewLookup();
        }

        protected override IMvxIosViewPresenter CreatePresenter()
        {
            Forms.Init();
            Mvx.LazyConstructAndRegisterSingleton<IViewLookupInitialiser, FormsViewLookupInitializer>();
            var xamarinFormsApp = new MvxFormsApplication();
            return new MvxFormsIosPagePresenter(Window, xamarinFormsApp);
        }

        protected override void InitializeLastChance()
        {
            base.InitializeLastChance();

            var config = Mvx.Resolve<IConfigurationService>().CurrentConfiguration;
            config.ApplicationInsightsAppId = IosApplicationInsightsAppId;
            config.ApplicationInsightsSecretKey = IosApplicationInsightsSecretKey;
        }
    }

}
