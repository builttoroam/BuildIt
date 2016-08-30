using System.Threading.Tasks;
using MvvmCross.Plugins.Network.Rest;

namespace BuildIt.MvvmCross.Data.Services
{
    public class JsonDownloadService:IDownloadService
    {
        
        private IMvxJsonRestClient RestClient { get; set; }

        public JsonDownloadService(IMvxJsonRestClient restClient)
        {
            RestClient = restClient;
        }

        public async Task<TResponse> RequestAsync<TResponse>(string uri)
        {
            return await RestClient.RequestAsync<TResponse>(uri);
        }

        public async  Task<TResponse> RequestAsync<TRequest, TResponse>(string uri, TRequest requestObject) where TRequest : class
        {
            return await RestClient.RequestAsync<TRequest, TResponse>(uri,requestObject);
        }
    }

    public interface IDownloadService
    {
        Task<TResponse> RequestAsync<TResponse>(string uri);
        Task<TResponse> RequestAsync<TRequest, TResponse>(string uri, TRequest requestObject) where TRequest : class;

    }
}