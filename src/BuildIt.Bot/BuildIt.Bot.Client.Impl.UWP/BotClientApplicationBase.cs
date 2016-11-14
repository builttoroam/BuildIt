using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using BuildIt.Bot.Client.Interfaces;
using BuildIt.Bot.Client.Models;
using BuildIt.Bot.Client.Services;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Utilities;

namespace BuildIt.Bot.Client.Impl.UWP
{
    public class BotClientApplicationBase : Application, IPushRegistrationService
    {
        private readonly EndpointRouteDetails endpointRouteDetails;

        public Func<string> RetrieveCurrentRegistrationId { private get; set; }

        public Action<string> RegistrationSuccessful { private get; set; }
        public Action<Exception> RegistrationFailure { private get; set; }

        public BotClientApplicationBase(EndpointRouteDetails endpointRouteDetails)
        {
            this.endpointRouteDetails = endpointRouteDetails;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            TryCleaningToastNotificationHistory();
        }

        protected async Task InitNotificationsAsync()
        {
            // Get a channel URI from WNS.
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
            channel.PushNotificationReceived += OnPushNotificationReceived;

            await RegisterForPushNotification(channel.Uri);
        }

        private void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            args.Cancel = true;
        }

        private async Task RegisterForPushNotification(string deviceToken)
        {
            try
            {
                var registration = new PushRegistration()
                {
                    Handle = deviceToken,
                    RegistrationId = RetrieveCurrentRegistrationId?.Invoke(),
                    Platform = PushPlatform.WNS
                };
                var botClientMobileAppClient = new BotClientMobileAppClient(endpointRouteDetails);
                var hubRegistrationResult = await botClientMobileAppClient.RegisterPushAsync(registration);
                if (hubRegistrationResult != null)
                {
                    RegistrationSuccessful?.Invoke(hubRegistrationResult.RegistrationId);
                }
            }
            catch (Exception ex)
            {
                RegistrationFailure?.Invoke(ex);
                Debug.WriteLine(ex.Message);
            }
        }

        private bool TryCleaningToastNotificationHistory()
        {
            try
            {
                var history = ToastNotificationManager.History;
                history.Clear();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return false;
        }
    }
}
