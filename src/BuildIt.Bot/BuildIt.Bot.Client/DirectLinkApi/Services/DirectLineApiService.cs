using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using BuildIt.Bot.Client.DirectLinkApi.Interfaces;
using BuildIt.Bot.Client.DirectLinkApi.Models;
using BuildIt.Bot.Client.Services.Interface;
using BuildIt.Bot.Client.Utilities;
using BuildIt.Web.Utilities;
using BuildIt.Web;
using Microsoft.Rest;

namespace BuildIt.Bot.Client.DirectLinkApi.Services
{
    public class DirectLineApiService : IDirectLineApiService
    {
        //
        // A Direct Line secret is a master key that can access any conversation, and create tokens. Secrets do not expire.
        // A Direct Line token is a key for a single conversation. It expires but can be refreshed.
        //
        private readonly IDirectLinkApiClient masterClient;

        // The token expires in 30 minutes and must be refreshed before then to remain useful.        
        private string conversationToken;

        private DateTimeOffset convoTokenExpirationDate;

        public const string TenantId = "BotConnector";

        public DirectLinkApiClient ConversationClient { get; private set; }
        public string ConversationId { get; private set; }

        public DirectLineApiService(IDirectLinkApiClient masterClient)
        {
            this.masterClient = masterClient;
        }

        public async Task<string> StartConversation()
        {
            if (string.IsNullOrWhiteSpace(ConversationId))
            {
                ConversationId = await StartNewConversation();
            }

            return ConversationId;
        }
        public void EndConversation()
        {
            ConversationId = null;
        }

        public async Task<bool> SendMessage(Message message)
        {
            var res = false;
            if (string.IsNullOrWhiteSpace(ConversationId))
            {
                Debug.WriteLine("You must call StartConversation() first");
                return res;
            }
            if (message == null) return res;

            try
            {
                if (await EnsureTokenIsValid())
                {
                    var postMessageResponse = await ConversationClient.Conversations.PostMessageWithOperationResponseAsync(ConversationId, message);
                    res = postMessageResponse?.Response?.StatusCode == HttpStatusCode.NoContent;
                }
                else
                {
                    //TODO: Handle situation when token is not valid
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return res;
        }

        private async Task<string> StartNewConversation()
        {
            var newConvoTokenResponse = await masterClient.Tokens.GenerateTokenForNewConversationWithOperationResponseAsync();
            if (!string.IsNullOrWhiteSpace(newConvoTokenResponse?.Body))
            {
                ConversationClient = new DirectLinkApiClient(new TokenCredentials(newConvoTokenResponse.Body, TenantId), new LoggingHttpHandler());
                var newConvoResponse = await ConversationClient.Conversations.NewConversationWithOperationResponseAsync();
                if (!string.IsNullOrWhiteSpace(newConvoResponse?.Body?.ConversationId))
                {
                    conversationToken = newConvoResponse.Body.Token;
                    convoTokenExpirationDate = DateTimeOffset.UtcNow.AddMinutes(Utilities.Constants.ConvoTokenExpirationTimeInMinutes);

                    return newConvoResponse.Body.ConversationId;
                }
                else
                {
                    //TODO handle new conversation creation failure
                }
            }
            else
            {
                //TODO handle generation of new conversation token failure
            }

            return null;
        }

        private async Task<bool> EnsureTokenIsValid()
        {
            var res = false;

            if (masterClient == null ||
                (string.IsNullOrWhiteSpace(conversationToken) || string.IsNullOrWhiteSpace(ConversationId))
                || DateTime.UtcNow > convoTokenExpirationDate || ConversationClient == null) return res;

            //MK renew only when getting close to end date (~5 min)
            if (DateTime.UtcNow.AddMinutes(-5) > convoTokenExpirationDate)
            {
                var refreshConvoTokenResponse = await ConversationClient.Tokens.RenewTokenWithOperationResponseAsync(ConversationId);
                if (!string.IsNullOrWhiteSpace(refreshConvoTokenResponse?.Body))
                {
                    conversationToken = refreshConvoTokenResponse.Body;
                    convoTokenExpirationDate = DateTimeOffset.UtcNow.AddMinutes(Utilities.Constants.ConvoTokenExpirationTimeInMinutes);
                    ConversationClient = new DirectLinkApiClient(new TokenCredentials(conversationToken, TenantId), new LoggingHttpHandler());
                    res = true;
                }
                else
                {
                    //TODO handle refresh of new conversation token failure
                }
            }
            else
            {
                res = true;
            }

            return res;
        }
    }
}
