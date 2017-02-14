using System;
using System.Reflection;
using BuildIt.Config.Core.Api.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using BuildIt.Config.Core.Api.Utilities;
using BuildIt.Config.Core.Models;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Swagger;

namespace BuildIt.Config.Web.Core.Sample
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            // Register your various configuration sources such as any json files, environment variables, etc.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(opts =>
            {
                opts.SwaggerDoc("v1", new Info { Title = "App Configuration API", Version = "v1" });
            });

            // Pass your completed configuration sources, and an optional route to initialise AppConfiguration
            services.AddBuildItConfig(Configuration, new AppConfigurationRoutingModel {Prefix = "api3", Controller = "test"});
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CreateEnvironmentVariables();

            app.UseSwagger();
            app.UseSwaggerUi(opts =>
            {
                opts.SwaggerEndpoint("/swagger/v1/swagger.json", "App Configuration API v1");
            });
            app.UseMvc();
            
        }

        private void CreateEnvironmentVariables()
        {
            //Environment.SetEnvironmentVariable("App_VersionInfo_CurrentAppVersion", "1.0.0");
            Environment.SetEnvironmentVariable("App_VersionInfo_MinimumAppVersion", "1.0.1");
            Environment.SetEnvironmentVariable("App_ServiceNotification_Title", "Some title");
            Environment.SetEnvironmentVariable("App_States", "[{\"FullName\":\"Scotland\",\"ShortCode\":\"SCO\",\"StateId\":1},{\"FullName\":\"North\",\"ShortCode\":\"NOR\",\"StateId\":13},{\"FullName\":\"Midlands\",\"ShortCode\":\"MID\",\"StateId\":14},{\"FullName\":\"Wales\",\"ShortCode\":\"WAL\",\"StateId\":4},{\"FullName\":\"South East\",\"ShortCode\":\"SEA\",\"StateId\":16},{\"FullName\":\"South West\",\"ShortCode\":\"SWE\",\"StateId\":17}]");

            Configuration.Reload();
        }
    }
}
