using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace BuildIt.Config.Web.Core.Sample
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
