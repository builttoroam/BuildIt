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
