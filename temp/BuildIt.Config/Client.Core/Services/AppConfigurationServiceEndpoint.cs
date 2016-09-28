using BuildIt.Config.Core.Standard.Services.Interfaces;

namespace Client.Core.Services
{
    public class AppConfigurationServiceEndpoint : IAppConfigurationServiceEndpoint
    {
        //public string Endpoint => "http://localhost:8459/test1/configuration";
        public string Endpoint => "http://fnmservices-dev.azurewebsites.net/test1/configuration";
    }
}
