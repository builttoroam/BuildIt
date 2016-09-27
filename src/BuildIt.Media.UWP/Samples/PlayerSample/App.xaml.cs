using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using BuildIt.Media;

namespace PlayerSample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private IActivatedEventArgs voiceActivation;
        private string voiceDefinitions;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);

            // Handle activation based on voice commands
            // var handled = await BuildIt.Player.Cortana.HandleActivation(args);
            // if (handled) return;

            //if (VoiceActivation !=null)
            //{
            //    if (VoiceActivation.Kind != ActivationKind.VoiceCommand) return;
            //}
            //else
            //{
            //    if (args.Kind != ActivationKind.VoiceCommand) return;
            //}
            //var commandArgs = args as VoiceCommandActivatedEventArgs;
            //var destination = commandArgs?.Result?.RulePath[0];
            //PlayerControls.Action(destination);

            await args.HandleMediaElementCortanaCommands();
        }

        //public IActivatedEventArgs VoiceActivation
        //{
        //    get { return voiceActivation; }
        //    set { voiceActivation = value; }
        //}

        //public string VoiceDefinitions
        //{
        //    get { return voiceDefinitions; }
        //    set { voiceDefinitions = value; }
        //}

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                //else
                //{
                //    if ((e.Arguments + "").ToLower().Contains("pause"))
                //    {
                //        PlayerControls.PauseMedia();
                //    }
                //}


                // Ensure the current window is active
                Window.Current.Activate();
            }

            // Registering for voice commands
            // BuildIt.Player.Cortana.RegisterVoiceCommands(@"VoiceDefinitions.xml");

       await CortanaListener.RegisterMediaElementVoiceCommands(@"CustomCortana.xml");
            //try
            //{
            //    StorageFile voices;
            //    if (string.IsNullOrEmpty(VoiceDefinitions))
            //    {
            //        voices = await Package.Current.InstalledLocation.GetFileAsync(@"Voices.xml");
            //    }
            //    else
            //    {
            //        voices = await Package.Current.InstalledLocation.GetFileAsync(VoiceDefinitions);
            //    }

            //    await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(voices);

            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("Installing Voice Commands Failed: " + ex.ToString());
            //}
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
