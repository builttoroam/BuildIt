﻿// Code generated by Microsoft (R) AutoRest Code Generator 0.9.7.0
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace BuildIt.Bot.Client.DirectLinkApi.Models
{
    /// <summary>
    /// 
    /// </summary>
    public partial class Message
    {
        /// <summary>
        /// Optional. Array of non-image attachments included in this message
        /// </summary>
        public IList<Attachment> Attachments { get; set; }

        /// <summary>
        /// Optional. Opaque block of data passed to/from bot via the
        /// ChannelData field
        /// </summary>
        public string ChannelData { get; set; }

        /// <summary>
        /// Optional. Conversation ID for this message
        /// </summary>
        public string ConversationId { get; set; }

        /// <summary>
        /// Optional. UTC timestamp when this message was created
        /// </summary>
        public DateTimeOffset? Created { get; set; }

        /// <summary>
        /// Optional.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Optional. Identity of the sender of this message
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// Optional. ID for this message
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Optional. Array of URLs for images included in this message
        /// </summary>
        public IList<string> Images { get; set; }

        /// <summary>
        /// Optional. Text in this message
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new instance of the Message class.
        /// </summary>
        public Message()
        {
            this.Attachments = new List<Attachment>();
            this.Images = new List<string>();
        }
        
        /// <summary>
        /// Deserialize the object
        /// </summary>
        public virtual void DeserializeJson(JToken inputObject)
        {
            if (inputObject != null && inputObject.Type != JTokenType.Null)
            {
                JToken attachmentsSequence = ((JToken)inputObject["attachments"]);
                if (attachmentsSequence != null && attachmentsSequence.Type != JTokenType.Null)
                {
                    foreach (JToken attachmentsValue in ((JArray)attachmentsSequence))
                    {
                        Attachment attachment = new Attachment();
                        attachment.DeserializeJson(attachmentsValue);
                        this.Attachments.Add(attachment);
                    }
                }
                JToken channelDataValue = inputObject["channelData"];
                if (channelDataValue != null && channelDataValue.Type != JTokenType.Null)
                {
                    this.ChannelData = channelDataValue.ToString(Newtonsoft.Json.Formatting.Indented);
                }
                JToken conversationIdValue = inputObject["conversationId"];
                if (conversationIdValue != null && conversationIdValue.Type != JTokenType.Null)
                {
                    this.ConversationId = ((string)conversationIdValue);
                }
                JToken createdValue = inputObject["created"];
                if (createdValue != null && createdValue.Type != JTokenType.Null)
                {
                    this.Created = ((DateTimeOffset)createdValue);
                }
                JToken eTagValue = inputObject["eTag"];
                if (eTagValue != null && eTagValue.Type != JTokenType.Null)
                {
                    this.ETag = ((string)eTagValue);
                }
                JToken fromValue = inputObject["from"];
                if (fromValue != null && fromValue.Type != JTokenType.Null)
                {
                    this.From = ((string)fromValue);
                }
                JToken idValue = inputObject["id"];
                if (idValue != null && idValue.Type != JTokenType.Null)
                {
                    this.Id = ((string)idValue);
                }
                JToken imagesSequence = ((JToken)inputObject["images"]);
                if (imagesSequence != null && imagesSequence.Type != JTokenType.Null)
                {
                    foreach (JToken imagesValue in ((JArray)imagesSequence))
                    {
                        this.Images.Add(((string)imagesValue));
                    }
                }
                JToken textValue = inputObject["text"];
                if (textValue != null && textValue.Type != JTokenType.Null)
                {
                    this.Text = ((string)textValue);
                }
            }
        }
        
        /// <summary>
        /// Serialize the object
        /// </summary>
        /// <returns>
        /// Returns the json model for the type Message
        /// </returns>
        public virtual JToken SerializeJson(JToken outputObject)
        {
            if (outputObject == null)
            {
                outputObject = new JObject();
            }
            JArray attachmentsSequence = null;
            if (this.Attachments != null)
            {
                //if (this.Attachments is ILazyCollection<Attachment> == false || ((ILazyCollection<Attachment>)this.Attachments).IsInitialized)
                if (this.Attachments is ICollection<Attachment> == false)
                {
                    attachmentsSequence = new JArray();
                    outputObject["attachments"] = attachmentsSequence;
                    foreach (Attachment attachmentsItem in this.Attachments)
                    {
                        if (attachmentsItem != null)
                        {
                            attachmentsSequence.Add(attachmentsItem.SerializeJson(null));
                        }
                    }
                }
            }
            if (this.ChannelData != null)
            {
                outputObject["channelData"] = JObject.Parse(this.ChannelData);
            }
            if (this.ConversationId != null)
            {
                outputObject["conversationId"] = this.ConversationId;
            }
            if (this.Created != null)
            {
                outputObject["created"] = this.Created.Value;
            }
            if (this.ETag != null)
            {
                outputObject["eTag"] = this.ETag;
            }
            if (this.From != null)
            {
                outputObject["from"] = this.From;
            }
            if (this.Id != null)
            {
                outputObject["id"] = this.Id;
            }
            JArray imagesSequence = null;
            if (this.Images != null)
            {
                //if (this.Images is ILazyCollection<string> == false || ((ILazyCollection<string>)this.Images).IsInitialized)
                if (this.Images is ICollection<string> == false)
                {
                    imagesSequence = new JArray();
                    outputObject["images"] = imagesSequence;
                    foreach (string imagesItem in this.Images)
                    {
                        if (imagesItem != null)
                        {
                            imagesSequence.Add(imagesItem);
                        }
                    }
                }
            }
            if (this.Text != null)
            {
                outputObject["text"] = this.Text;
            }
            return outputObject;
        }
    }
}
