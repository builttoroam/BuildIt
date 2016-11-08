using System.Threading.Tasks;
using BuildIt.Bot.Client.DirectLinkApi.Models;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    public interface IDirectLineApiService : IDirectLineApiConversationProvider
    {
        Task<string> StartConversation();
        void EndConversation();

        Task<bool> SendMessage(Message message);
    }
}
