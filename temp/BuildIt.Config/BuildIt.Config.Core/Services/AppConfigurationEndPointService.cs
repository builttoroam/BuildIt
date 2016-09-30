using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildIt.Config.Core.Standard.Models;
using BuildIt.Config.Core.Standard.Services.Interfaces;

namespace BuildIt.Config.Core.Standard.Services
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
