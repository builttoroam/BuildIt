using BuildIt.Config.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;


namespace BuildIt.Config.Core.Api.Controllers
{

    public class AppConfigurationController : Controller
    {

        [HttpPost]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public AppConfigurationServerResponse Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, [FromQuery]string hash = null)
        {
            if (configMapperValues == null) return null;

            var res = new AppConfigurationServerResponse();

            var config = Environment.GetEnvironmentVariables();

            foreach (var configMapperValue in configMapperValues)
            {
                if (configMapperValue.ValueIsRequired && !config.Contains(configMapperValue.Name))
                {
                    if (res.AppConfigErors == null) res.AppConfigErors = new List<AppConfigurationError>();
                    var appConfigError = new AppConfigurationError
                    {
                        Content = Constants.AppConfigurationKeyNotFoundError,
                        Mapping = configMapperValue
                    };
                    res.AppConfigErors.Add(appConfigError);
                }
                else
                {
                    var appConfigValue = new AppConfigurationValue()
                    {
                        Value = config[configMapperValue.Name] as string,
                        Attributes = configMapperValue
                    };
                    res.AppConfigValues.Add(appConfigValue);
                }
            }

            //MK don't return app configuration values if an error occured
            if (res.HasErrors) res.AppConfigValues = null;
            return res;
        }
    }
}
