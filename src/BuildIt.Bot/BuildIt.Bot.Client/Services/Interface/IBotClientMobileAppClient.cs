using System.Threading.Tasks;
using BuildIt.Web.Models;

namespace BuildIt.Bot.Client.Services.Interface
{
    public interface IBotClientMobileAppClient
    {
        Task<HubRegistrationResult> RegisterPushAsync(PushRegistration pushRegistration);

        Task DeregisterPushAsync(PushRegistration pushRegistration);
    }
}
