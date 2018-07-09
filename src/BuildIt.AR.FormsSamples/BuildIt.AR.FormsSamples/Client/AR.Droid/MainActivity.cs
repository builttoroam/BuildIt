using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using Xamarin.Forms;
using MvvmCross.Core.Views;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using HockeyApp.Android;
using MvvmCross.Forms.Core;
using MvvmCross.Forms.Droid;
using MvvmCross.Forms.Droid.Presenters;

//using BuildIt.AR.FormsSamples.Core;

namespace BuildIt.AR.FormsSamples.Android
{
    [Activity(Label = "BuildIt.AR.FormsSamples", Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            Forms.Init(this, bundle);
            //SetContentView(Resource.Layout.activity_camera);

            // Set up mvvmcross
            var mvxFormsApp = new MvxFormsApplication();
            LoadApplication(mvxFormsApp);

            //Add HockeyApp authentication
            //if (!string.Equals(SharedConstants.HockeyApp.AndroidSecretKey, SharedConstants.HockeyApp.AndroidSecretKey))
            //{
            //    LoginManager.Register(this, SharedConstants.HockeyApp.AndroidSecretKey, LoginManager.LoginModeEmailPassword);
            //    LoginManager.VerifyLogin(this, Intent);
            //}

            //var xformsApp = new App();

            var presenter = Mvx.Resolve<IMvxViewPresenter>() as MvxFormsDroidPagePresenter;
            presenter.FormsApplication = mvxFormsApp;
            //presenter.MvxFormsApp = mvxFormsApp;
            Mvx.Resolve<IMvxAppStart>().Start();
            if (bundle != null)
            {
                //FragmentManager.BeginTransaction().Replace(Resource.Id.container, Camera2BasicFragment.NewInstance()).Commit();
            }
        }
        protected override void OnResume()
        {
            base.OnResume();
            //Add HockeyApp crash reporting
            var config = Mvx.Resolve<IConfigurationService>().CurrentConfiguration;
            if (!string.IsNullOrWhiteSpace(config.ApplicationInsightsAppId) &&
                        config.ApplicationInsightsAppId != Guid.Empty.ToString())
            {
                CrashManager.Register(this, config.ApplicationInsightsAppId);
            }
        }
    }
}