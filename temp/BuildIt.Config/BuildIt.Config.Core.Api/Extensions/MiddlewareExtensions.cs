using Microsoft.Extensions.DependencyInjection;

namespace BuildIt.Config.Core.Api.Extensions
{
    public static class MiddlewareExtensions
    {
        public static void AddAppConfiguration(this IServiceCollection services)
        {
            if (services == null) return;

            //var dc = DependencyContext.Default;
            //foreach (var compilationLibrary in dc.CompileLibraries)
            //{
            //    //TODO: Find better way of finding assembly name
            //    if (compilationLibrary.Name.Contains("BuildIt.Config.Core.Api"))
            //    {
            //        Assembly.Load(new AssemblyName(compilationLibrary.Name));
            //    }
            //}
        }

        //public static void UseAppConfiguration(this IApplicationBuilder app, string controller = null, string method = null)
        //{
        //    app.Use(async (context, next) =>
        //    {
        //        if (context.Request.IsHttps) //Before RC1, this was called 
        //        {
        //            await next();
        //        }
        //        else
        //        {
        //            var withHttps = "https://" + context.Request.Host + context.Request.Path;
        //            context.Response.Redirect(withHttps);
        //        }
        //    });
        //}
    }
}
