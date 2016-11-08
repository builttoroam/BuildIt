using System;
using BuildIt.Bot.Client.DirectLinkApi.Models;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    public interface IMessageManager
    {
        event EventHandler<Message> MessageReceived;

        void StartListening();
    }
}
