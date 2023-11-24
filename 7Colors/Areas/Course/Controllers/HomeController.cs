using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Course.Controllers
{
    [Area("Course")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
