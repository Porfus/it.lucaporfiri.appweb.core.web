using Microsoft.AspNetCore.Mvc;

namespace it.lucaporfiri.appweb.core.web.Controllers
{
    public class BaseController : Controller
    {
        public TimeZoneInfo InfoTime { get; set; }
        public BaseController()
        {
            // Set the time zone to Central European Standard Time (CET)
            InfoTime = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
