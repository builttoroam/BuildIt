using System.Web;
using System.Web.Mvc;

namespace BuildItConfigSample.WebFullFramework
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
