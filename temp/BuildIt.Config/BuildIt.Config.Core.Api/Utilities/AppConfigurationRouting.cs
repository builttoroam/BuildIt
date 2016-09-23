using System.Threading.Tasks;
using BuildIt.Config.Core.Api.Controllers;
using Microsoft.AspNetCore.Routing;

namespace BuildIt.Config.Core.Api.Utilities
{
    public class AppConfigurationRouting : IRouter
    {
        public Task RouteAsync(RouteContext context)
        {
            throw new System.NotImplementedException();
        }

        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
