using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ImagesController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHost;

        public ImagesController(AppDbContext context, IWebHostEnvironment webHost)
        {
            this.context = context;
            this.webHost = webHost;
        }

        public IActionResult Index(int? id = null)
        {
            if (id == null)
            {
                return View(context.Images.Include(p => p.Post).ToList());
            }
            else
            {
                return View(context.Images.Where(i=>i.PostId == id)
                    .Include(p => p.Post).ToList());
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(context.Posts.ToList(), "Id", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Image img, [FromForm] IFormFile Url)
        {
            var existedImg = context.Images.FirstOrDefault(i => i.Title == img.Title & i.PostId == img.PostId);
            if (existedImg != null)
            {
                ViewBag.message = "هذه الصورة موجودة مسبقاً";
                ViewData["PostId"] = new SelectList(context.Posts.ToList(), "Id", "Title");
                return View(img);
            }
            if (Url != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images", Path.GetFileName(Url.FileName));
                Url.CopyTo(new FileStream(name, FileMode.Create));
                img.Url = "/images/" + Url.FileName;
            }
            if (Url == null)
            {
                img.Url = "/images/noimage.PNG";
            }
            if (ModelState.IsValid)
            {
                context.Images.Add(img);
                await context.SaveChangesAsync();
                TempData["Create"] = "لقد تم إضافة الصورة";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(context.Posts.ToList(), "Id", "Title");
            return View(img);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewData["PostId"] = new SelectList(context.Posts.ToList(), "Id", "Title");
            if (id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Post).FirstOrDefault(c => c.Id == id);
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Image img, IFormFile Url)
        {
            if (Url != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images", Path.GetFileName(Url.FileName));
                await Url.CopyToAsync(new FileStream(name, FileMode.Create));
                img.Url = "/images/" + Url.FileName;
            }
            if (ModelState.IsValid)
            {
                context.Images.Update(img);
                await context.SaveChangesAsync();
                TempData["Edit"] = "لقد تم تعديل الصورة";
                return RedirectToAction(nameof(Index));
            }
            return View(img);
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Post).FirstOrDefault(c => c.Id == id);
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Post).FirstOrDefault(c => c.Id == id);
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Image img)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != img.Id)
            {
                return NotFound();
            }
            var p = context.Images.FirstOrDefault(c => c.Id == id);
            if (p == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.Images.Remove(p);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف الصورة";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        #region MyRegion

        #endregion
    }
}
