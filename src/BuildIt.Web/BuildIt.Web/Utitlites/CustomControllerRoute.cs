#if NET45

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BuildIt.Web.Controller;
using BuildIt.Web.Models.Routing;

namespace BuildIt.Web.Utitlites
{
    /// <summary>
    /// 
    /// </summary>
    public class CustomControllerRoute<T> : RouteBase where T : System.Web.Mvc.Controller
    {
        private readonly CustomControllerRoutingModel<T> routingModel;        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="routingModel"></param>        
        public CustomControllerRoute(CustomControllerRoutingModel<T> routingModel = null)
        {
            this.routingModel = routingModel ?? CustomControllerRoutingModel<T>.Default;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // IMPORTANT: Always return null if there is no match.
            // This tells .NET routing to check the next route that is registered.
            if (!IsCustomRoute(httpContext.Request.RawUrl)) return null;

            try
            {
                var result = new RouteData(this, new MvcRouteHandler());
                result.Values["controller"] = typeof(T).Name.Replace(nameof(Controller), "");
                result.Values["action"] = "Post";

                return result;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="values"></param>
        /// <returns></returns>
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