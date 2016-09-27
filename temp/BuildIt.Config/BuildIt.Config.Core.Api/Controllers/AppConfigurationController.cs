using System;
using System.Collections.Generic;
using BuildIt.Config.Core;
using BuildIt.Config.Core.Standard.Models;
#if NETStandard16
using Microsoft.AspNetCore.Mvc;
#elif NET452
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
        public IEnumerable<AppConfigurationValue> Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, string hash = null)
#elif NET452
        public JsonResult Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, string hash = null)
#endif
        {
            if (configMapperValues == null) return null;

            var res = new AppConfiguration();

            var config = Environment.GetEnvironmentVariables();

            foreach (var configMapperValue in configMapperValues)
            {
                res[configMapperValue.Name] = new AppConfigurationValue()
                {
                    //TODO: Change this after referencing BuildIt.Core to SafeDictValue()
                    Value = config.Contains(configMapperValue.Name) ? config[configMapperValue.Name] as string : null,
                    Attributes = configMapperValue
                };
            }
#if NETStandard16
            return res.Values;
#elif NET452
            return new JsonResult()
            {
                Data = res.Values
            };
#endif
        }
    }

}
