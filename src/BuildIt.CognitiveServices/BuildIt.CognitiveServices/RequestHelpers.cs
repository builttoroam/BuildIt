using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Rest;
using Newtonsoft.Json;

namespace BuildIt.CognitiveServices
{
    public static class RequestHelpers
    {
        public static async Task<TResult> Request<TClient, TResult>(this TClient serviceClient, Func<TClient, Task<HttpOperationResponse>> service) where TClient : ServiceClient<TClient>
        {
            var result = await service(serviceClient);

            var stream = await result.Response.Content.ReadAsStreamAsync();
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<TResult>(jsonTextReader);
            }
        }
    }
}
