using System.Threading.Tasks;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;

namespace BuildIt.Bot.Client.Services.Interface
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBotClientMobileAppClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        Task<HubRegistrationResult> RegisterPushAsync(PushRegistration pushRegistration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        Task DeregisterPushAsync(PushRegistration pushRegistration);
    }
}
