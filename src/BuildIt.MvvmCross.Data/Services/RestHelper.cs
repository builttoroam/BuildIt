using MvvmCross.Plugins.Network.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BuildIt.MvvmCross.Data.Services
{
    public static class JsonRestHelper
    {
        public static async Task<TResponse> RequestAsync<TResponse>(this IMvxJsonRestClient restClient, string uri)
        {
            return await restClient.RequestAsync<object, TResponse>(uri, null);
        }
        public static async Task<TResponse> RequestAsync<TRequest, TResponse>(this IMvxJsonRestClient restClient, string uri, TRequest requestObject) where TRequest : class
        {
            try
            {
                var request =
                    new MvxJsonRestRequest<TRequest>(uri);
                //var waiter = new ManualResetEvent(false);
                //TResponse responseValue = default(TResponse);
                //Exception raisedException=null;
                var response = await restClient.MakeRequestForAsync<TResponse>(request);
                //    response =>
                //    {
                //        responseValue = response.Result;
                //        Debug.WriteLine(response != null);
                //        // do something with the response.StatusCode and response.Result
                //        waiter.Set();
                //    },
                //    error =>
                //    {
                //        raisedException = error;
                //        // do something with the error
                //        waiter.Set();
                //    });

                //await Task.Run(() => waiter.WaitOne());

                //if (raisedException != null) throw raisedException;
                return response.Result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}