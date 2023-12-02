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

        public IActionResult Index()
        {
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public IActionResult New()
        {
            var viewModel = new MessageViewModel() { TimeSend= DateTime.Now};

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageViewModel message)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var ms = new Message()
                {
                    Email = message.Email,
                    Content = message.Content,
                    PhoneNumber = message.PhoneNumber,
                    Name = message.Name,
                    TimeSend = DateTime.Now,
                };
                _context.Messages.Add(ms);
                await _context.SaveChangesAsync();
                return Json(new { IsSuccess = "redirect", description = Url.Action("Home", "Index", new { id = message.Id }), message });
            }

        }                        
        
    }
}
