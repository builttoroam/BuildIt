using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using BuildIt.Config.Core.Api.Utilities;
using BuildIt.Config.Core.Models;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.Swagger;

namespace BuildIt.Config.Web.Core.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAppConfiguration();      
            //services.AddRouting();
            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new Info { Title = "App Configuration API", Version = "v1" });
            });

            services.AddMvc(opts =>
            {
                opts.Conventions.Insert(0, new AppConfigurationRoutingConvention(new AppConfigurationRoutingModel() { Prefix = "api3", Controller = "test" }));
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                CreateEnvironmentVariables();
            }

            app.UseSwagger();
            app.UseMvc();
            
        }

        private void CreateEnvironmentVariables()
        {
            //Environment.SetEnvironmentVariable("App_VersionInfo_CurrentAppVersion", "1.0.0");
            Environment.SetEnvironmentVariable("App_VersionInfo_MinimumAppVersion", "1.0.1");
            Environment.SetEnvironmentVariable("App_ServiceNotification_Title", "Some title");
            Environment.SetEnvironmentVariable("App_States", "[{\"FullName\":\"Scotland\",\"ShortCode\":\"SCO\",\"StateId\":1},{\"FullName\":\"North\",\"ShortCode\":\"NOR\",\"StateId\":13},{\"FullName\":\"Midlands\",\"ShortCode\":\"MID\",\"StateId\":14},{\"FullName\":\"Wales\",\"ShortCode\":\"WAL\",\"StateId\":4},{\"FullName\":\"South East\",\"ShortCode\":\"SEA\",\"StateId\":16},{\"FullName\":\"South West\",\"ShortCode\":\"SWE\",\"StateId\":17}]");
        }
    }
}
