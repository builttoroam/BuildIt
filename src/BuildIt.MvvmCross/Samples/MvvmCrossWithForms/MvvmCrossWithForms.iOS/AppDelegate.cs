using System;
using System.Collections.Generic;
using System.Linq;
using BuildIt.MvvmCross.Forms.iOS;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Forms.iOS;
using MvvmCross.Platform;
using MvvmCrossWithForms.Core;
using UIKit;

namespace MvvmCrossWithForms.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : CustomMvxFormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            var setup = new SetupFromViewModel<MainViewModel, MvvmCrossWithForms.App>(this, Window);
            setup.Initialize();

            MvxLoadApplication();

            Window.MakeKeyAndVisible();


            //global::Xamarin.Forms.Forms.Init();
            //LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
