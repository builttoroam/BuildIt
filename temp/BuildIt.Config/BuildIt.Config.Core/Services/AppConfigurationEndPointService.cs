using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Core.Services
{
    public class AppConfigurationEndpointService : IAppConfigurationEndpointService
    {
        private readonly AppConfigurationRoutingModel routingModel;

        public string Endpoint => $"{routingModel.BaseUrl}/{routingModel.Prefix}/{routingModel.Controller}";

        public AppConfigurationEndpointService(AppConfigurationRoutingModel routingModel)
        {
            this.routingModel = routingModel;
        }
    }
}
