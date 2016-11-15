using System;
using BuildIt.Bot.Client.DirectLinkApi.Models;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// 
        /// </summary>
        event EventHandler<Message> MessageReceived;

        /// <summary>
        /// 
        /// </summary>
        void StartListening();
    }
}
