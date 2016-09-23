using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using BuildIt.Config.Core;
using BuildIt.Config.Core.Api.Models;

namespace BuildIt.Config.Core.Api.Controllers
{
    [Route("api/[controller]")]
    public class AppConfigurationController : Controller
    {
        public AppConfigurationController()
        {

        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "b", "a" };
        }

        //[Route("app/{hash?}")]
        [HttpPost]
        public IEnumerable<AppConfigurationValue> Post([FromBody]List<AppConfigurationMapperAttributes> configMapperValues, string hash = null)
        {
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

            return res.Values;
        }
    }

}
