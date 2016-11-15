using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using BuildIt.Bot.Client.Models;
using BuildIt.Bot.Client.Services.Interface;
using BuildIt.Web;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Models.Results;
using BuildIt.Web.Utilities;
using Newtonsoft.Json;

namespace BuildIt.Bot.Client.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class BotClientMobileAppClient : IBotClientMobileAppClient
    {
        private readonly EndpointRouteDetails endpointRouteDetails;

        /// <summary>
        /// 
        /// </summary>
        public bool IsMobileService { get; set; } = true;

        /// <summary>
        /// Route to the server is build from BaseServiceUrl, ServiceAffix and registerPushRoute variables, specified in the constructor.
        /// The route by default could look like this: [your_base_url]/api/registerpush
        /// </summary>        
        /// <param name="endpointRouteDetails"></param>
        public BotClientMobileAppClient(EndpointRouteDetails endpointRouteDetails)
        {
            this.endpointRouteDetails = endpointRouteDetails;
        }

        /// <summary>
        /// Method which, by making a call to the service, registers your device to push notifications hub.        
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task<HubRegistrationResult> RegisterPushAsync(PushRegistration pushRegistration)
        {
            if (endpointRouteDetails?.BaseServiceUrl == null) return null;

            HubRegistrationResult hubRegistrationResult = null;
            try
            {
                var json = JsonConvert.SerializeObject(pushRegistration);
                using (var client = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{endpointRouteDetails.BaseServiceUrl}/{endpointRouteDetails.ServiceAffix}/{endpointRouteDetails.RegisterPushRoute}");
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
                    var request = new HttpRequestMessage(HttpMethod.Post, $"{endpointRouteDetails.BaseServiceUrl}/{endpointRouteDetails.ServiceAffix}/{endpointRouteDetails.DeregisterPushRoute}");
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
