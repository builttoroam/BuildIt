using System;
using Android.App;
using Android.Content;
using Android.OS;
using BuildIt.Bot.Client.Impl.Droid.Utilities;
using Gcm.Client;
using Constants = BuildIt.Bot.Client.Impl.Droid.Utilities.Constants;

namespace BuildIt.Bot.Client.Impl.Droid
{
    /// <summary>
    /// All of the activities MUST derive from this class
    /// </summary>
    public abstract class BotClientActivityBase : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            TryCleaningToastNotificationHistory();
        }

        protected override void OnResume()
        {
            base.OnResume();

            Settings.Instance.IsAppInForeground = true;
        }

        protected override void OnPause()
        {
            base.OnPause();

            Settings.Instance.IsAppInForeground = false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitNotificationsAsync(string googleApiConsoleAppProjectNumber)
        {
            try
            {
                //Check to see that GCM is supported and that the manifest has the correct information
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                //Call to Register the device for Push Notifications
                GcmClient.Register(this, googleApiConsoleAppProjectNumber);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        private bool IsLaunchedByPushNotification()
        {
            return Intent.GetBooleanExtra(Constants.PushNotificationExtra, false);
        }

        private bool TryCleaningToastNotificationHistory()
        {
            try
            {
                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
                if (notificationManager != null)
                {
                    notificationManager.CancelAll();
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            return false;
        }
    }
}