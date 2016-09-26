using System.Web.Http;
using System.Web.Mvc;

namespace Web.FullFramework.Controllers
{
    public class HomeController : Controller
    {
        [System.Web.Http.HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
