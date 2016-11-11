﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BuildIt.Bot.Client.DirectLinkApi.Models
{
    public partial class MessageSet
    {
        /// <summary>
        /// Optional.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Optional. Messages
        /// </summary>
        public IList<Message> Messages { get; set; }

        /// <summary>
        /// Optional. Maximum watermark included in this set of messages
        /// </summary>
        public string Watermark { get; set; }

        /// <summary>
        /// Initializes a new instance of the MessageSet class.
        /// </summary>
        public MessageSet()
        {
            this.Messages = new List<Message>();
        }
        
        /// <summary>
        /// Deserialize the object
        /// </summary>
        public virtual void DeserializeJson(JToken inputObject)
        {
            if (inputObject != null && inputObject.Type != JTokenType.Null)
            {
                JToken eTagValue = inputObject["eTag"];
                if (eTagValue != null && eTagValue.Type != JTokenType.Null)
                {
                    this.ETag = ((string)eTagValue);
                }
                JToken messagesSequence = ((JToken)inputObject["messages"]);
                if (messagesSequence != null && messagesSequence.Type != JTokenType.Null)
                {
                    foreach (JToken messagesValue in ((JArray)messagesSequence))
                    {
                        Message message = new Message();
                        message.DeserializeJson(messagesValue);
                        this.Messages.Add(message);
                    }
                }
                JToken watermarkValue = inputObject["watermark"];
                if (watermarkValue != null && watermarkValue.Type != JTokenType.Null)
                {
                    this.Watermark = ((string)watermarkValue);
                }
            }
        }
    }
}