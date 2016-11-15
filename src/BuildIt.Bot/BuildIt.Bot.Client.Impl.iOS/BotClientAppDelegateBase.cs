using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildIt.Bot.Client.Interfaces;
using BuildIt.Bot.Client.Models;
using BuildIt.Bot.Client.Services;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Utilities;
using Foundation;
using UIKit;

namespace BuildIt.Bot.Client.Impl.iOS
{
    public abstract class BotClientAppDelegateBase : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IPushRegistrationService
    {
        private readonly EndpointRouteDetails endpointRouteDetails;

        public Func<string> RetrieveCurrentRegistrationId { private get; set; }

        public Action<string> RegistrationSuccessful { private get; set; }
        public Action<Exception> RegistrationFailure { private get; set; }

        protected BotClientAppDelegateBase(EndpointRouteDetails endpointRouteDetails)
        {
            this.endpointRouteDetails = endpointRouteDetails;
        }

        protected void InitNotificationsAsync(string googleApiConsoleAppProjectNumber)
        {
            RegisterForPushNotifications();

            TryCleaningToastNotificationHistory();
        }

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            var trimmedDeviceToken = deviceToken?.Description;
            if (string.IsNullOrWhiteSpace(trimmedDeviceToken)) return;

            trimmedDeviceToken = trimmedDeviceToken.Trim('<').Trim('>');
            trimmedDeviceToken = trimmedDeviceToken.Replace(" ", string.Empty);

            await RegisterForPushNotifications(trimmedDeviceToken);
        }
        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            //base.FailedToRegisterForRemoteNotifications(application, error);
            RegistrationFailure?.Invoke(new Exception(error.ToString()));
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            TryCleaningToastNotificationHistory();            
        }

        private void RegisterForPushNotifications()
        {
            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }

        private async Task RegisterForPushNotifications(string deviceToken)
        {
            try
            {
                var botClientMobileApp = new BotClientMobileAppClient(endpointRouteDetails);
                var registration = new PushRegistration()
                {
                    Handle = deviceToken,
                    RegistrationId = RetrieveCurrentRegistrationId?.Invoke(),
                    Platform = PushPlatform.APNS,
                };
                var hubRegistrationResult = await botClientMobileApp.RegisterPushAsync(registration);
                if (hubRegistrationResult != null)
                {
                    RegistrationSuccessful?.Invoke(deviceToken);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                RegistrationFailure?.Invoke(ex);
            }
        }

        private bool TryCleaningToastNotificationHistory()
        {
            try
            {
                //MK Clearing push notification from notification center http://stackoverflow.com/a/9225972/510627
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 1;
                UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
                UIApplication.SharedApplication.CancelAllLocalNotifications();

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return false;
        }
    }
}