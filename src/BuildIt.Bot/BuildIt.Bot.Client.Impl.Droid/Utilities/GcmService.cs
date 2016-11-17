using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using BuildIt.Bot.Client.Impl.Droid.Extensions;
using BuildIt.Bot.Client.Interfaces;
using BuildIt.Bot.Client.Services;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Utilities;
using Gcm.Client;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace BuildIt.Bot.Client.Impl.Droid.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    [Service] //Must use the service tag
    public class GcmService : GcmServiceBase
    {
        private readonly BotClientMobileAppClient botClientMobileApp;

        /// <summary>
        /// 
        /// </summary>
        public GcmService()
            : base(PushHandlerBroadcastReceiver.GoogleApiConsoleAppProjectNumber)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(Settings.Instance.EndpointRouteDetails?.BaseServiceUrl))
            {
                throw new Exception("You need to set the BaseServiceUrl, in Settings.Instance.EndpointRouteDetails, before working with Push Notifications");
            }
#endif
            this.botClientMobileApp = new BotClientMobileAppClient(Settings.Instance.PushNotificationSettings.EndpointRouteDetails);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceToken"></param>
        protected override async void OnRegistered(Context context, string deviceToken)
        {
            //Receive registration Id for sending GCM Push Notifications to
            try
            {
                await RegisterPushNotifications(deviceToken);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessagingCenter.Send(this, Constants.FailureSubscriptionMsg, ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="registrationId"></param>
        protected override void OnUnRegistered(Context context, string registrationId)
        {
            //Receive notice that the app no longer wants notifications
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        protected override void OnMessage(Context context, Intent intent)
        {
            try
            {
                Debug.WriteLine("Push Notification received!");
                if (!Settings.Instance.IsAppInForeground)
                {
                    var contentAvailable = intent?.Extras.GetString(Constants.ContentAvailable);
                    if (!string.IsNullOrWhiteSpace(contentAvailable))
                    {
                    }
                    else
                    {
                        ProcessPushNotification(intent);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorId"></param>
        /// <returns></returns>
        protected override bool OnRecoverableError(Context context, string errorId)
        {
            //Some recoverable error happened
            MessagingCenter.Send(this, Constants.FailureSubscriptionMsg, new Exception(errorId));

            return base.OnRecoverableError(context, errorId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="errorId"></param>
        protected override void OnError(Context context, string errorId)
        {
            MessagingCenter.Send(this, Constants.FailureSubscriptionMsg, new Exception(errorId));
            //Some more serious error happened
        }

        private async Task RegisterPushNotifications(string deviceToken)
        {
            try
            {
                var registration = new PushRegistration()
                {
                    Handle = deviceToken,
                    RegistrationId = Settings.Instance.RegistrationId,
                    Platform = PushPlatform.GCM
                };
                var hubRegistrationResult = await botClientMobileApp.RegisterPushAsync(registration);
                if (hubRegistrationResult != null)
                {
                    MessagingCenter.Send(this, Constants.SuccessSubscriptionMsg, hubRegistrationResult.RegistrationId);
                }
                else
                {
                    MessagingCenter.Send(this, Constants.FailureSubscriptionMsg, new Exception("Registration Failure"));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                MessagingCenter.Send(this, Constants.FailureSubscriptionMsg, ex);
            }
        }

        private void ProcessPushNotification(Intent intent)
        {
            var title = intent?.Extras.GetStringForLowercaseKey("Title");
            var body = intent?.Extras.GetStringForLowercaseKey("Body");
            if (!string.IsNullOrWhiteSpace(title) && !string.IsNullOrWhiteSpace(body))
            {
                DisplayPushNotification(title, body);
            }
        }

        private void DisplayPushNotification(string title, string body)
        {
            Intent intent = new Intent(Application.Context, Settings.Instance.PushNotificationSettings.MainActivityType);
            intent.PutExtra(Constants.PushNotificationExtra, true);
            intent.SetFlags(Settings.Instance.PushNotificationSettings.ActivityFlags);
            // Create a PendingIntent; we're only using one PendingIntent (ID = 0):
            const int pendingIntentId = 0;
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
            var notificationBuilder = new Notification.Builder(this).SetContentTitle(title)
                                                                    .SetContentText(body)
                                                                    .SetAutoCancel(true)
                                                                    .SetContentIntent(pendingIntent);
            if (Settings.Instance.PushNotificationSettings?.SmallIcon.HasValue ?? false)
            {
                notificationBuilder.SetSmallIcon(Settings.Instance.PushNotificationSettings.SmallIcon.Value);
            }

            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.Notify(new Random().Next(100000), notificationBuilder.Build());
        }
    }
}