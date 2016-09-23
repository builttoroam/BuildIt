using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Routing;

namespace BuildIt.Config.Core.Api.Utilities
{
    public class CustomControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template { get; }
        public int? Order { get; }
        public string Name { get; }
    }
}
