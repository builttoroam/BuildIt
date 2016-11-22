using System;
using System.Linq;
using System.Collections.Generic;


using BuildIt.Config.Core;
using BuildIt.Config.Core.Models;
#if NETStandard16

using Microsoft.AspNetCore.Mvc;
#elif NET452
using System.Web.UI;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
#endif

namespace BuildIt.Config.Core.Api.Controllers
{

#if NETStandard16
    public class AppConfigurationController : Controller
#elif NET452    
    public class AppConfigurationController : Controller
#endif
    {


#if NETStandard16
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public AppConfigurationServerResponse Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, string hash = null)
#elif NET452
        [OutputCache(NoStore = true, Location = OutputCacheLocation.None)]
        public JsonResult Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, string hash = null)
#endif
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

#if NETStandard16
            return res;
#elif NET452
            return new JsonResult()
            {
                Data = res
            };
#endif
        }
    }

}
