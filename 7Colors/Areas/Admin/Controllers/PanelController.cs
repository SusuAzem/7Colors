using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class PanelController : Controller
    {
                
            public IActionResult Index()
            {
                return View();
            }

            public IActionResult FrontPage()
            {
                return View();
            }

        
    }
}
