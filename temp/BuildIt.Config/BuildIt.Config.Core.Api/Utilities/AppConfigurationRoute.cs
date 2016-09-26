#if NET452
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using BuildIt.Config.Core.Api.Controllers;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Api.Utilities
{
    public class AppConfigurationRoute : RouteBase
    {
        private readonly AppConfigurationRoutingModel routingModel;

        public AppConfigurationRoute(AppConfigurationRoutingModel routingModel = null)
        {
            this.routingModel = routingModel ?? AppConfigurationRoutingModel.Default;
        }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // IMPORTANT: Always return null if there is no match.
            // This tells .NET routing to check the next route that is registered.
            if (!IsCustomRoute(httpContext.Request.RawUrl)) return null;

            var result = new RouteData(this, new MvcRouteHandler());
            result.Values["controller"] = nameof(AppConfigurationController).Replace(nameof(Controller), "");
            result.Values["action"] = "Post";            

            return result;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            //// IMPORTANT: Always return null if there is no match.
            //// This tells .NET routing to check the next route that is registered.
            if (!IsCustomRoute(requestContext.HttpContext.Request.RawUrl)) return null;

            return new VirtualPathData(this, null);
        }

        private bool IsCustomRoute(string url)
        {
            return url.ToLower().Contains($"{routingModel.Prefix}/{routingModel.Controller}");
        }
    }
}
#endif