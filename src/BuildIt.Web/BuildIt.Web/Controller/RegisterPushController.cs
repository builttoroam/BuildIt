#if NET45

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using BuildIt.Web.Interfaces;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Models.Results;
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
        public async Task<JsonResult> Post(PushRegistration pushRegistration)
        {
            var res = new HubRegistrationResult();
            try
            {
                res.RegistrationId = await notificationService.CreateOrUpdateRegistrationAsync(pushRegistration);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                res.ErrorMessage = ex.Message;
            }

            return Json(res);
        }
    }
}

#endif