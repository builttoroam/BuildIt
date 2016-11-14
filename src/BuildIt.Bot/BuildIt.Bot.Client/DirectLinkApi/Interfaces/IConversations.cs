﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System.Threading;
using System.Threading.Tasks;
using BuildIt.Bot.Client.DirectLinkApi.Models;
using Microsoft.Rest;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public partial interface IConversations
    {
        /// <param name='conversationId'>
        /// Required. Conversation ID
        /// </param>
        /// <param name='watermark'>
        /// Optional. (Optional) only returns messages newer than this watermark
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<MessageSet>> GetMessagesWithOperationResponseAsync(string conversationId, string watermark = null, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<Conversation>> NewConversationWithOperationResponseAsync(CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='conversationId'>
        /// Required. Conversation ID
        /// </param>
        /// <param name='message'>
        /// Required. Message to send
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> PostMessageWithOperationResponseAsync(string conversationId, Message message, CancellationToken cancellationToken = default(System.Threading.CancellationToken));
        
        /// <param name='conversationId'>
        /// Required.
        /// </param>
        /// <param name='cancellationToken'>
        /// Cancellation token.
        /// </param>
        Task<HttpOperationResponse<object>> UploadWithOperationResponseAsync(string conversationId, CancellationToken cancellationToken = default(System.Threading.CancellationToken));        
    }
}
