#if NET46

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using BuildIt.Web.Interfaces;
using BuildIt.Web.Models;
using BuildIt.Web.Utilities;
using BuildIt.Bot.Api.Models;

namespace BuildIt.Bot.Api.Dialogs
{
    [Serializable]
    public abstract class PushNotificationDialog<T> : IDialog<T>
    {
        private readonly INotificationService notificationService;
        private readonly ConversationDetails conversationDetails;

        private IDialogContext baseContext;

        public PushNotificationDialog(INotificationService notificationService, ConversationDetails conversationDetails)
        {
            this.notificationService = notificationService;
            this.conversationDetails = conversationDetails;
        }

        protected abstract Task StartWithPushNotifications();

#pragma warning disable 1998
        public virtual async Task StartAsync(IDialogContext context)
#pragma warning restore 1998
        {
            baseContext = context;
            await StartWithPushNotifications();
        }

        protected async Task PostAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            try
            {
                await baseContext.PostAsync(message);
                await notificationService.SendPushNotificationAsync(new PushNotification()
                {
                    Title = "There are new messages in your conversation",
                    Body = message,
                    Platforms = PushPlatform.WNS | PushPlatform.GCM
                }, conversationDetails.ConversationId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        protected void Wait(ResumeAfter<IMessageActivity> resumeAfter)
        {
            baseContext.Wait(resumeAfter);
        }

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

#endif