using System.Net.Http.Headers;
using BuildIt.AR.FormsSamples.Models;

namespace BuildIt.AR.FormsSamples.Client
{
    public class BearerAuthenticationHeaderValue : AuthenticationHeaderValue
    {
        private const string Bearer = "bearer";

        public BearerAuthenticationHeaderValue(string accessToken) : base(Bearer, accessToken)
        {
        }
    }
}
