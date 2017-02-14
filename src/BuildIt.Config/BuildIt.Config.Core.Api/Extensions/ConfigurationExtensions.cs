using BuildIt.Config.Core.Api.Utilities;
using BuildIt.Config.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildIt.Config.Core.Api.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddBuildItConfig(this IServiceCollection services, IConfiguration config,
            AppConfigurationRoutingModel route = null)
        {
            services.AddSingleton(sp => config);

            services.AddMvc(opts =>
            {
                opts.Conventions.Insert(0, new AppConfigurationRoutingConvention(route));
            });

            return services;
        }
    }
}
