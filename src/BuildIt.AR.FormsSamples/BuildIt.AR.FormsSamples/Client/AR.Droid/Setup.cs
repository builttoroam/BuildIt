using Android.Content;
using BuildIt.AR.FormsSamples.Core;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using BuildIt.AR.FormsSamples.UI;
using MvvmCross.Core.ViewModels;
using MvvmCross.Core.Views;
using MvvmCross.Droid.Platform;
using MvvmCross.Droid.Views;
using MvvmCross.Forms.Droid.Presenters;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;

namespace BuildIt.AR.FormsSamples.Android
{
    public class Setup : MvxAndroidSetup
    {
        public const string AndroidApplicationInsightsAppId = "";
        public const string AndroidApplicationInsightsSecretKey = "";

        public Setup(Context applicationContext)
            : base(applicationContext)
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

        protected override IMvxAndroidViewPresenter CreateViewPresenter()
        {
            Mvx.LazyConstructAndRegisterSingleton<IViewLookupInitialiser, FormsViewLookupInitializer>();

            var presenter = new MvxFormsDroidPagePresenter();
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
            config.ApplicationInsightsAppId = AndroidApplicationInsightsAppId;
            config.ApplicationInsightsSecretKey = AndroidApplicationInsightsSecretKey;
        }

    }

}
