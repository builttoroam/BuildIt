using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BuildIt.Web.Utilities
{
    public class LoggingHttpHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                Debug.WriteLine($"Request URI  {request.RequestUri}");
                Debug.WriteLine($"Request Method {request.Method}");
                Debug.WriteLine($"Request Headers {request.Headers}");
                if (request.Content != null)
                {
                    Debug.WriteLine($"Request Content {await request.Content.ReadAsStringAsync()}");
                    Debug.WriteLine($"Request Content-Headers {request.Content.Headers}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
