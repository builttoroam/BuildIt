using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BuildIt.Web.Interfaces;
using BuildIt.Web.Models;
using BuildIt.Web.Utilities;
using BuildIt.Bot.Api.Models;
using BuildIt.Web.Models.PushNotifications;

namespace BuildIt.Bot.Api.Dialogs
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class PushNotificationDialog<T> : IDialog<T>
    {
        private readonly INotificationService notificationService;
        private readonly ConversationDetails conversationDetails;
        private readonly PushNotificationDetails pushNotificationDetails;

        private IDialogContext baseContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notificationService"></param>
        /// <param name="conversationDetails"></param>
        /// <param name="pushNotificationDetails"></param>
        protected PushNotificationDialog(INotificationService notificationService, ConversationDetails conversationDetails, PushNotificationDetails pushNotificationDetails)
        {
            this.notificationService = notificationService;
            this.conversationDetails = conversationDetails;
            this.pushNotificationDetails = pushNotificationDetails;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected abstract Task StartWithPushNotifications();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
#pragma warning disable 1998
        public virtual async Task StartAsync(IDialogContext context)
#pragma warning restore 1998
        {
            baseContext = context;
            await StartWithPushNotifications();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected async Task PostAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            try
            {
                await baseContext.PostAsync(message);
                await notificationService.SendPushNotificationAsync(new PushNotification()
                {
                    Title = pushNotificationDetails.PushNotificationTitle,
                    Body = message,
                    Platforms = pushNotificationDetails.SupportedPushPlatforms,
                    PushNotificationLaunchArgument = pushNotificationDetails.PushNotificationLaunchArgument
                }, conversationDetails.ConversationId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resumeAfter"></param>
        protected void Wait(ResumeAfter<IMessageActivity> resumeAfter)
        {
            baseContext.Wait(resumeAfter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        protected async Task ResumeAfterWithDone(IDialogContext context, IAwaitable<T> result)
        {
            var res = default(T);
            try
            {
                res = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Done(res);
            }
        }
    }
}

