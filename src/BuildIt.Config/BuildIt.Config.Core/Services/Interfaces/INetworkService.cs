using System.Threading.Tasks;

namespace BuildIt.Config.Core.Services.Interfaces
{
    public interface INetworkService
    {
        bool HasInternetConnection();
    }
}
