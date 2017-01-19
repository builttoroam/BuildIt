using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using BuildIt.Bot.Client.Impl.Droid.Models;
using BuildIt.Bot.Client.Impl.Droid.Utilities;
using BuildIt.Bot.Client.Interfaces;
using Gcm.Client;
using Xamarin.Forms;
using Constants = BuildIt.Bot.Client.Impl.Droid.Utilities.Constants;
using Debug = System.Diagnostics.Debug;

namespace BuildIt.Bot.Client.Impl.Droid
{
    /// <summary>
    /// All of the activities MUST derive from this class
    /// </summary>
    public abstract class BotClientActivityBase : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPushRegistrationService
    {
        /// <summary>
        /// 
        /// </summary>
        public Func<string> RetrieveCurrentRegistrationId { private get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Action<string> RegistrationSuccessful { private get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Action<Exception> RegistrationFailure { private get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundle"></param>
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            TryCleaningToastNotificationHistory();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnResume()
        {
            base.OnResume();

            try
            {
                MessagingCenter.Subscribe<GcmService, string>(this, Utilities.Constants.SuccessSubscriptionMsg, (service, s) => { RegistrationSuccessful?.Invoke(s); });
                MessagingCenter.Subscribe<GcmService, Exception>(this, Utilities.Constants.FailureSubscriptionMsg, (service, e) => { RegistrationFailure?.Invoke(e); });

                Settings.Instance.IsAppInForeground = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnPause()
        {
            base.OnPause();

            try
            {
                MessagingCenter.Unsubscribe<GcmService, string>(this, Utilities.Constants.SuccessSubscriptionMsg);
                MessagingCenter.Unsubscribe<GcmService, Exception>(this, Utilities.Constants.FailureSubscriptionMsg);

                Settings.Instance.IsAppInForeground = false;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitNotifications(PushNotificationSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings?.EndpointRouteDetails?.BaseServiceUrl) || string.IsNullOrWhiteSpace(settings.GoogleApiConsoleAppProjectNumber) || settings.MainActivityType == null)
            {
#if DEBUG
                Debug.WriteLine("You need to specify all the necessary settings before you initializing push notifications");
#endif
                return;
            }

            try
            {
                Settings.Instance.RegistrationId = RetrieveCurrentRegistrationId?.Invoke();
                Settings.Instance.PushNotificationSettings = settings;

                //Check to see that GCM is supported and that the manifest has the correct information
                GcmClient.CheckDevice(this);
                GcmClient.CheckManifest(this);

                //Call to Register the device for Push Notifications
                GcmClient.Register(this, settings.GoogleApiConsoleAppProjectNumber);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private bool IsLaunchedByPushNotification()
        {
            return Intent.GetBooleanExtra(Utilities.Constants.PushNotificationExtra, false);
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
                Debug.WriteLine(e);
            }

            return false;
        }
    }
}