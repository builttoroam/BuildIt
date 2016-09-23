﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BuildIt.Config.Core.Api.Controllers;
using BuildIt.Config.Core.Api.Models;
using BuildIt.Config.Core.Api.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Newtonsoft.Json;

namespace BuildIt.Config.Core.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        //public static void AddAppConfiguration(this IServiceCollection services)
        //{
        //    if (services == null) return;

        //    var dc = DependencyContext.Default;
        //    foreach (var compilationLibrary in dc.CompileLibraries)
        //    {
        //        //TODO: Find better way of finding assembly name
        //        if (compilationLibrary.Name.Contains("BuildIt.Config.Core.Api"))
        //        {
        //            Assembly.Load(new AssemblyName(compilationLibrary.Name));
        //        }
        //    }
        //}

        //public static void UseAppConfiguration(this IApplicationBuilder app, AppConfigurationRoutingModel appConfigurationRouting = null)
        //{
            //appConfigurationRouting = appConfigurationRouting ?? AppConfigurationRoutingModel.Default;
            //var defaultHandler = app.ApplicationServices.GetRequiredService<MvcRouteHandler>();
            //var routeBuilder = new RouteBuilder(app, defaultHandler);
            //var routeBuilder = new RouteBuilder(app, new RouteHandler(async (context) =>
            //{
            //    using (var ms = new MemoryStream())
            //    {
            //        try
            //        {
            //            if (context.Request.Body.CanSeek) context.Request.Body.Position = 0;

            //            context.Request.Body.CopyTo(ms);

            //            var bodyString = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            //            var appConfigurationController = new AppConfigurationController();
            //            var appConfiguration = appConfigurationController.Post(JsonConvert.DeserializeObject<List<AppConfigurationMapperAttributes>>(bodyString));
            //            var appConfigurationJson = JsonConvert.SerializeObject(appConfiguration);

            //            await context.Response.WriteAsync(appConfigurationJson);
            //        }
            //        catch (Exception ex)
            //        {
            //            Debug.WriteLine(ex.Message);
            //        }
            //    }
            //}));
            ////MK http://codeclimber.net.nz/archive/2008/11/14/how-to-call-controllers-in-external-assemblies-in-an-asp.net.aspx
            //routeBuilder.MapRoute("App Configuration",
            //                      $"{appConfigurationRouting.Prefix}/{{controller={Constants.DefaultAppConfigurationControllerName}}}");

            //var routes = routeBuilder.Build();
            //app.UseRouter(routes);
        //}
    }
}
