using System.Threading.Tasks;
using BuildIt.Web.Models;
using BuildIt.Web.Models.PushNotifications;

namespace BuildIt.Web.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        Task<string> CreateOrUpdateRegistrationAsync(PushRegistration pushRegistration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushRegistration"></param>
        /// <returns></returns>
        Task DeleteRegistrationAsync(PushRegistration pushRegistration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pushNotification"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        Task SendPushNotificationAsync(PushNotification pushNotification, params string[] tags);
    }
}
