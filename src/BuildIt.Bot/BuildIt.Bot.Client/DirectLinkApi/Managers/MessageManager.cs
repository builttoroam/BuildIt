using System;
using System.Linq;
using System.Threading.Tasks;
using BuildIt.Bot.Client.DirectLinkApi.Interfaces;
using BuildIt.Bot.Client.DirectLinkApi.Models;

namespace BuildIt.Bot.Client.DirectLinkApi.Managers
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageManager : IMessageManager
    {
        private readonly IDirectLineApiConversationProvider conversationProvider;

        private string messageReceivedWatermark;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<Message> MessageReceived;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="conversationProvider"></param>
        public MessageManager(IDirectLineApiConversationProvider conversationProvider)
        {
            this.conversationProvider = conversationProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        public void StartListening()
        {
            if (string.IsNullOrWhiteSpace(conversationProvider?.ConversationId) || conversationProvider.ConversationClient == null) return;

            Task.Run(async () =>
            {
                while (true)
                {
                    var messagesResponse = await conversationProvider.ConversationClient.Conversations.GetMessagesWithOperationResponseAsync(conversationProvider.ConversationId, messageReceivedWatermark);
                    if (messagesResponse?.Body?.Messages?.Any() ?? false)
                    {
                        messageReceivedWatermark = messagesResponse.Body.Watermark;
                        foreach (var message in messagesResponse.Body.Messages)
                        {
                            MessageReceived?.Invoke(this, message);
                        }
                    }

                    await Task.Delay(500);
                }
            });
        }
    }
}
