using BuildIt.Lifecycle;
using StateByState.Services;
using StateByState.Shared;
using StateByState.Shared.Views;
using System;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Navigation;
using StateByState.Regions.Main;
using StateByState.Regions.Secondary;

namespace StateByState
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            // Associate main region states with corresponding native pages
            this.RegisterView<MainPage>().ForState(MainRegionView.Main);
            this.RegisterView<SecondPage>().ForState(MainRegionView.Second);
            LifecycleHelper.RegisterView<ThirdPage>().ForState(MainRegionView.Third);
            LifecycleHelper.RegisterView<FourthPage>().ForState(MainRegionView.Fourth);

            // Register the sub-pages of the third page
            LifecycleHelper.RegisterView<ThrirdOnePage>().ForState(ThirdStates.One);
            LifecycleHelper.RegisterView<ThirdTwoPage>().ForState(ThirdStates.Two);
            LifecycleHelper.RegisterView<ThirdThreePage>().ForState(ThirdStates.Three);
            LifecycleHelper.RegisterView<ThirdFourPage>().ForState(ThirdStates.Four);

            // Register the page to be used in the additional window
            LifecycleHelper.RegisterView<SeparatePage>().ForState(SecondaryRegionView.Main);

            var core = new SampleApplication();
            var wm = new WindowManager(core);
            await core.Startup(builder =>
            {
                builder.Register<UWPSpecial, ISpecial>();
            });
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}