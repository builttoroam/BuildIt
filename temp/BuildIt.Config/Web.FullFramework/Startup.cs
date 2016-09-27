using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Owin;
using Owin;
using BuildIt.Config.Core.Api.Controllers;

[assembly: OwinStartup(typeof(Web.FullFramework.Startup))]

namespace Web.FullFramework
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ControllerBuilder.Current.DefaultNamespaces.Add("BuildIt.Config.Core.Api.Controllers");                                            

            Environment.SetEnvironmentVariable("App_VersionInfo_CurrentAppVersion", "AAAA");

            ConfigureAuth(app);
        }
    }
}
