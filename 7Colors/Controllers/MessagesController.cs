using _7Colors.Data;
using _7Colors.Models;
using _7Colors.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Controllers
{  
    public class MessagesController : Controller
    {
        private readonly AppDbContext _context;

        public MessagesController(AppDbContext context)
        {
            _context = context;
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public IActionResult New()
        {
            var viewModel = new MessagesViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Message message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Messages.Add(message);
                await _context.SaveChangesAsync();
                return Json(new { IsSuccess = "redirect", description = Url.Action("Home", "Index", new { id = message.Id }), message });
            }

        }
    }
}
