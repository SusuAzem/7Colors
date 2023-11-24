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

        public IActionResult Index(int? Id = null)
        {
            if (Id == null)
            {
                ViewData["type"] = "allG";
                return View(context.Images.Include(p => p.Group).ToList());
            }
            else
            {
                ViewData["type"] = context.HPGroups.FirstOrDefault(g=>g.Id == Id)!.Title;
                return View(context.Images.Where(i=>i.GroupId == Id).ToList());
            }
        }
      
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(context.HPGroups.ToList(), "Id", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Image img, [FromForm] IFormFile Url)
        {
            var existedImg = context.Images.FirstOrDefault(i => i.Title == img.Title & i.GroupId == img.GroupId);
            if (existedImg != null)
            {
                ViewBag.message = "هذه الصورة موجودة مسبقاً";
                ViewData["GroupId"] = new SelectList(context.HPGroups.ToList(), "Id", "Title");
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
            ViewData["GroupId"] = new SelectList(context.HPGroups.ToList(), "Id", "Title");
            return View(img);
        }


        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            ViewData["GroupId"] = new SelectList(context.HPGroups.ToList(), "Id", "Title");
            if (Id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Group).FirstOrDefault(c => c.Id == Id);
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
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Group).FirstOrDefault(c => c.Id == Id);
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Image img)
        {
            return RedirectToAction(nameof(Edit));
        }

        [HttpGet]
        public IActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var img = context.Images.Include(c => c.Group).FirstOrDefault(c => c.Id == Id);
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, Image img)
        {
            if (Id == null)
            {
                return NotFound();
            }
            if (Id != img.Id)
            {
                return NotFound();
            }
            var p = context.Images.FirstOrDefault(c => c.Id == Id);
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
