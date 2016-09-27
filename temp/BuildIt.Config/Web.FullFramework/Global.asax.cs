using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BuildIt.Config.Core.Api.Utilities;
using BuildIt.Config.Core.Standard.Models;
using Web.FullFramework.Models;

namespace Web.FullFramework
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //var r = new Route("test/config", new AppConfigurationRoutingHandler());
            //RouteTable.Routes.Add(r);
            RouteTable.Routes.Add(new AppConfigurationRoute(new AppConfigurationRoutingModel()
            {
                Prefix = "test1",
                Controller = "configuration"
            }));
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
