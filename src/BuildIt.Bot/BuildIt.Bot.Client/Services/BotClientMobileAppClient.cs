using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BuildIt.Bot.Client.Services.Interface;
using BuildIt.Web;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Utilities;
using Newtonsoft.Json;

namespace BuildIt.Bot.Client.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BotClientMobileAppClient : IBotClientMobileAppClient
    {
        private readonly string baseServiceUrl;
        private readonly string serviceAffix;
        private readonly string registerPushRoute;
        private readonly string deregisterPushRoute;

        /// <summary>
        /// 
        /// </summary>
        public bool IsMobileService { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseServiceUrl"></param>
        /// <param name="serviceAffix"></param>
        /// <param name="registerPushRoute"></param>
        /// <param name="deregisterPushRoute"></param>
        public BotClientMobileAppClient(string baseServiceUrl, string serviceAffix = "api", string registerPushRoute = "registerpush", string deregisterPushRoute = "deregisterpush")
        {
            this.baseServiceUrl = baseServiceUrl;
            this.serviceAffix = serviceAffix;
            this.registerPushRoute = registerPushRoute;
            this.deregisterPushRoute = deregisterPushRoute;
        }

        /// <summary>
        /// Method which, by making a call to the service, registers your device to push notifications.
        /// Route to the server is build from baseServiceUrl, serviceAffix and registerPushRoute variables, specified in the constructor.
        /// The route by default could look like this: <your_base_url>/api/registerpush
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task<HubRegistrationResult> RegisterPushAsync(PushRegistration pushRegistration)
        {
            HubRegistrationResult hubRegistrationResult = null;
            try
            {
                var json = JsonConvert.SerializeObject(pushRegistration);
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{baseServiceUrl}/{serviceAffix}/{registerPushRoute}");
                    request.Content = new StringContent(json);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.JsonMimeType);
                    if (IsMobileService) request.Content.Headers.Add("ZUMO-API-VERSION", "2.0.0");
                    var response = await client.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        var contentJson = await response.Content.ReadAsStringAsync();
                        hubRegistrationResult = JsonConvert.DeserializeObject<HubRegistrationResult>(contentJson);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return hubRegistrationResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task DeregisterPushAsync(PushRegistration pushRegistration)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(pushRegistration);
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{baseServiceUrl}/{serviceAffix}/{deregisterPushRoute}");
                    request.Content = new StringContent(json);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(Constants.JsonMimeType);
                    if (IsMobileService) request.Content.Headers.Add("ZUMO-API-VERSION", "2.0.0");
                    var response = await client.SendAsync(request);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
