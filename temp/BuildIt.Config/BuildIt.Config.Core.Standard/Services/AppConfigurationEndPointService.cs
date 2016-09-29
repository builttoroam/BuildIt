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
        private AppConfigurationRoutingModel routingModel;
        private AppConfigurationRoutingModel RoutingModel
        {
            get
            {
                if (routingModel == null)
                {
                    routingModel = AppConfigurationRoutingModel.Default; ;
                }
                return routingModel;
            }
            set { routingModel = value; }
        }

        public string Endpoint => $"http://fnmservices-dev.azurewebsites.net/{RoutingModel.Prefix}/{RoutingModel.Controller}";

        public AppConfigurationEndpointService()
        {
            
        }
        public AppConfigurationEndpointService(AppConfigurationRoutingModel routingModel)
        {
            RoutingModel = routingModel;
        }
    }
}
