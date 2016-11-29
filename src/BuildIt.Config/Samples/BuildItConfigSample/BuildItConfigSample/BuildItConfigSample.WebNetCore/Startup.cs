using System;
using BuildIt.Config.Core.Api.Utilities;
using BuildIt.Config.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BuildItConfigSample.WebNetCore
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc(opts => { opts.Conventions.Insert(0, new AppConfigurationRoutingConvention(new AppConfigurationRoutingModel() { Prefix = "test1", Controller = "configuration" })); });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                CreateEnvironmentVariables();
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
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
