﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using Microsoft.Rest;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public partial interface IDirectLinkApiClient : IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        Uri BaseUri
        {
            get; set; 
        }
        
        /// <summary>
        /// Credentials for authenticating with the service.
        /// </summary>
        ServiceClientCredentials Credentials
        {
            get; set; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        IConversations Conversations
        {
            get; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        ITokens Tokens
        {
            get; 
        }
    }
}
