
#if NET45

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using BuildIt.Web.Interfaces;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Models.Routing;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace BuildIt.Web.Controller
{
    /// <summary>
    /// 
    /// </summary>        
    //public class RegisterPushController : ApiController
    public class RegisterPushController : System.Web.Mvc.Controller
    {
        private readonly INotificationService notificationService;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationService"></param>
        public RegisterPushController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> Post(PushRegistration pushRegistration)
        {
            string registrationId = null;
            try
            {
                registrationId = await notificationService.CreateOrUpdateRegistrationAsync(pushRegistration);
            }
            catch (MessagingException e)
            {
                var webex = e.InnerException as WebException;
                if (webex?.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)webex.Response;
                    if (response.StatusCode == HttpStatusCode.Gone)
                    {
                        // registration id was deleted or expired so try again
                        registrationId = await notificationService.CreateOrUpdateRegistrationAsync(pushRegistration);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return null;

            //catch (Exception ex)
            //{
            //    return InternalServerError(ex);
            //}
            //if (registrationId != null)
            //{
            //    return Ok(new HubRegistrationResult()
            //    {
            //        RegistrationId = registrationId
            //    });
            //}
            //return BadRequest("Could not register user for push notifications");
        }
    }
}

#endif