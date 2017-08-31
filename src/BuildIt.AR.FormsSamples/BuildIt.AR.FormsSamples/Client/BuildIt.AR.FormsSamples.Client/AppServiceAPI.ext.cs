using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BuildIt.AR.FormsSamples.Client
{
    public partial class AppServiceAPI
    {
        public AppServiceAPI(
            string baseUri,
            //string accessToken,
            params DelegatingHandler[] handlers) : this(new Uri(baseUri), handlers)
        {
            //HttpClient.DefaultRequestHeaders.Authorization = new BearerAuthenticationHeaderValue(accessToken);

        }
    }
}
