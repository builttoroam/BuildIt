using BuildIt.Config.Core.Services.Interfaces;

namespace Client.Core.Services
{
    public class AppConfigurationServiceEndpoint : IAppConfigurationServiceEndpoint
    {
        public string Endpoint => "http://localhost:11032/api/appconfiguration/app";
    }
}
