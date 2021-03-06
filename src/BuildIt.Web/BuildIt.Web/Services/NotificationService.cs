﻿#if NET45

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BuildIt.Web.Models;
using BuildIt.Web.Utilities;
using BuildIt.Web.Extensions;
using BuildIt.Web.Interfaces;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;
using BuildIt.Web.Utitlites;
using BuildIt.Web.Models.PushNotifications;
using BuildIt.Web.Models.Results;

namespace BuildIt.Web.Services
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class NotificationService : INotificationService
    {
        private readonly string hubConnectionString;
        private readonly string hubName;

        private NotificationHubClient notificationHub
        {
            get
            {
                try
                {
                    return NotificationHubClient.CreateClientFromConnectionString(hubConnectionString, hubName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hubConnectionString"></param>
        /// <param name="hubName"></param>
        public NotificationService(string hubConnectionString, string hubName)
        {
            this.hubConnectionString = hubConnectionString;
            this.hubName = hubName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task<string> CreateOrUpdateRegistrationAsync(PushRegistration pushRegistration)
        {
            UserRegistrationResult userRegistrationResult = await RegisterUserAsync(pushRegistration);
            if (userRegistrationResult == null)
            {
                return pushRegistration.RegistrationId;
            }

            RegistrationDescription registration = null;
            switch (pushRegistration.Platform)
            {
                case PushPlatform.APNS:
                    registration = new AppleRegistrationDescription(pushRegistration.Handle);
                    break;
                case PushPlatform.GCM:
                    registration = new GcmRegistrationDescription(pushRegistration.Handle);
                    break;
                case PushPlatform.WNS:
                    registration = new WindowsRegistrationDescription(pushRegistration.Handle);
                    break;
                default:
                    //throw new HttpResponseException(HttpStatusCode.BadRequest);
                    throw new Exception();
            }

            registration.RegistrationId = userRegistrationResult.RegistrationId;
            if (pushRegistration.Tags?.Any() ?? false)
            {
                registration.Tags = new HashSet<string>(pushRegistration.Tags);
            }

            await notificationHub.CreateOrUpdateRegistrationAsync(registration);

            return registration.RegistrationId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        public async Task DeleteRegistrationAsync(PushRegistration pushRegistration)
        {
            try
            {
                await notificationHub.DeleteRegistrationAsync(pushRegistration.RegistrationId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushNotification"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public async Task SendPushNotificationAsync(PushNotification pushNotification, params string[] tags)
        {
            if (string.IsNullOrWhiteSpace(pushNotification?.Title)) return;

            try
            {
                var settings = new JsonSerializerSettings { ContractResolver = new LowerCaseContractResolver() };
                var platforms = pushNotification.Platforms.GetFlags<PushPlatform>();
                foreach (var platform in platforms)
                {
                    switch (platform)
                    {
                        case PushPlatform.APNS:
                            var apnsAlert = $"{{ \"aps\" : {{ \"alert\" : {{ \"title\" : \"{pushNotification.Title}\", \"body\" : \"{pushNotification?.Body}\" }}}}}}";
                            await notificationHub.SendAppleNativeNotificationAsync(apnsAlert, tags);
                            break;
                        case PushPlatform.GCM:
                            var simplePushNotificationMessage = new
                            {
                                Title = pushNotification.Title,
                                Body = pushNotification.Body
                            };
                            var gcmAlert = $"{{ \"data\" : {JsonConvert.SerializeObject(simplePushNotificationMessage, settings)} }}";
                            await notificationHub.SendGcmNativeNotificationAsync(gcmAlert, tags);
                            break;
                        case PushPlatform.WNS:
                            var toast = $"<toast launch=\"{pushNotification?.PushNotificationLaunchArgument}\"><visual><binding template=\"ToastText02\"><text id=\"1\">{pushNotification.Title }</text><text id=\"2\">{ pushNotification.Body }</text></binding></visual></toast>";
                            await notificationHub.SendWindowsNativeNotificationAsync(toast, tags);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task<UserRegistrationResult> RegisterUserAsync(PushRegistration pushRegistration)
        {
            UserRegistrationResult userRegistrationResult = null;

            if (notificationHub == null) return null;

            string newRegistrationId = null;
            if (pushRegistration.Handle != null)
            {
                var registrations = await notificationHub.GetRegistrationsByChannelAsync(pushRegistration.Handle, 100);
                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await notificationHub.DeleteRegistrationAsync(registration);
                    }
                }
            }
            if (newRegistrationId == null)
            {
                var registrationId = await notificationHub.CreateRegistrationIdAsync();
                userRegistrationResult = new UserRegistrationResult() { RegistrationId = registrationId };
            }
            else
            {
                userRegistrationResult = new UserRegistrationResult() { RegistrationId = newRegistrationId };
            }
            return userRegistrationResult;
        }
    }
}
#endif
