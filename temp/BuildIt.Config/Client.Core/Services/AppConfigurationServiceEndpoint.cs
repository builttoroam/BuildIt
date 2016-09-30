using BuildIt.Config.Core.Services.Interfaces;

namespace Client.Core.Services
{
    public class AppConfigurationServiceEndpoint : IAppConfigurationEndpointService
    {
        //public string Endpoint => "http://localhost:8459/test1/configuration";
        public string Endpoint => "http://localhost:11032/api3/test";
    }
}
