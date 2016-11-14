using System.Threading.Tasks;
using BuildIt.Bot.Client.DirectLinkApi.Models;

namespace BuildIt.Bot.Client.DirectLinkApi.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDirectLineApiService : IDirectLineApiConversationProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<string> StartConversation();
        /// <summary>
        /// 
        /// </summary>
        void EndConversation();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> SendMessage(Message message);
    }
}
