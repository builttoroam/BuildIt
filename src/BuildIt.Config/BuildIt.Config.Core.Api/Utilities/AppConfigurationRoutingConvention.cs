using System.Linq;
using System.Text;
using BuildIt.Config.Core.Api.Controllers;
using BuildIt.Config.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BuildIt.Config.Core.Api.Utilities
{
    public class AppConfigurationRoutingConvention : IApplicationModelConvention
    {
        private readonly AppConfigurationRoutingModel routingModel;

        public AppConfigurationRoutingConvention(AppConfigurationRoutingModel routingModel)
        {
            this.routingModel = routingModel ?? AppConfigurationRoutingModel.Default;
        }

        public void Apply(ApplicationModel application)
        {
            var centralPrefix = new AttributeRouteModel(new RouteAttribute($"{routingModel.Prefix}/{routingModel.Controller}"));
            foreach (var controller in application.Controllers)
            {
                if (controller.ControllerType.UnderlyingSystemType == typeof(AppConfigurationController))
                {
                    var hasRouteAttributes = controller.Selectors.Any(selector => selector.AttributeRouteModel != null);
                    if (hasRouteAttributes)
                    {
                        // This controller manually defined some routes, so treat this 
                        // as an override and not apply the convention here.
                        continue;
                    }


                    foreach (var selector in controller.Selectors)
                    {
                        selector.AttributeRouteModel = centralPrefix;
                    }
                }
            }
        }
    }
}
