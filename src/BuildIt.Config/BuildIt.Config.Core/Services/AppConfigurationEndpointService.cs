using BuildIt.Config.Core.Models;
using BuildIt.Config.Core.Services.Interfaces;

namespace BuildIt.Config.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AppConfigurationEndpointService : IAppConfigurationEndpointService
    {
        private readonly AppConfigurationRoutingModel routingModel;

        /// <summary>
        /// 
        /// </summary>
        public string Endpoint => $"{routingModel.BaseUrl}/{routingModel.Prefix}/{routingModel.Controller}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routingModel"></param>
        public AppConfigurationEndpointService(AppConfigurationRoutingModel routingModel)
        {
            this.routingModel = routingModel;
        }
    }
}
