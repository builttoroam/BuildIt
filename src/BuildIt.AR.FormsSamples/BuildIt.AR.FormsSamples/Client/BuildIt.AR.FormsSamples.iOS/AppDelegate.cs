using System;
using BuildIt.AR.FormsSamples.Core.Services.Interfaces;
using Foundation;
using HockeyApp.iOS;
using UIKit;
using MvvmCross.iOS.Platform;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;

namespace BuildIt.AR.FormsSamples.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        private UIWindow window;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //Hocky App integration
            //if (!string.Equals(SharedConstants.HockeyApp.IosAppId, SharedConstants.HockeyApp.IosAppId))
            //{
            //    var manager = BITHockeyManager.SharedHockeyManager;
            //    manager.Configure(SharedConstants.HockeyApp.IosAppId);
            //    manager.StartManager();
            //    manager.Authenticator.AuthenticateInstallation();
            //}

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new Setup(this, window);
            setup.Initialize();

            var config = Mvx.Resolve<IConfigurationService>().CurrentConfiguration;
            if (!string.IsNullOrWhiteSpace(config.ApplicationInsightsAppId) &&
                        config.ApplicationInsightsAppId != Guid.Empty.ToString())
            {
                var manager = BITHockeyManager.SharedHockeyManager;
                manager.Configure(config.ApplicationInsightsAppId);
                manager.StartManager();
                manager.Authenticator.AuthenticateInstallation();
            }

            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();

            window.MakeKeyAndVisible();
            return true;
        }
    }
}
