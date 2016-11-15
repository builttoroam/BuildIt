﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using Newtonsoft.Json.Linq;

namespace BuildIt.Bot.Client.DirectLinkApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Conversation
    {
        /// <summary>
        /// Optional. ID for this conversation
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Optional. Token scoped to this conversation
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Initializes a new instance of the Conversation class.
        /// </summary>
        public Conversation()
        {
        }
        
        /// <summary>
        /// Deserialize the object
        /// </summary>
        public virtual void DeserializeJson(JToken inputObject)
        {
            if (inputObject != null && inputObject.Type != JTokenType.Null)
            {
                JToken conversationIdValue = inputObject["conversationId"];
                if (conversationIdValue != null && conversationIdValue.Type != JTokenType.Null)
                {
                    this.ConversationId = ((string)conversationIdValue);
                }
                JToken eTagValue = inputObject["eTag"];
                if (eTagValue != null && eTagValue.Type != JTokenType.Null)
                {
                    this.ETag = ((string)eTagValue);
                }
                JToken tokenValue = inputObject["token"];
                if (tokenValue != null && tokenValue.Type != JTokenType.Null)
                {
                    this.Token = ((string)tokenValue);
                }
            }
        }
    }
}
