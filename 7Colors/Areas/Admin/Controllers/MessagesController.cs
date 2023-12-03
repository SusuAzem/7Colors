using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var messages = _context.Messages.ToList();
            return Json(new { data = messages });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var message = _context.Messages.FirstOrDefault(m => m.Id == id);
            if (message == null)
            {
                return Json(new { success = false, message = "خطأ بعملية الحذف" });
            }

            _context.Messages.Remove(message);
            _context.SaveChanges();

            return Json(new { success = true, message = "تم الحذف بنجاح" });
        }

        #endregion

    }
}
