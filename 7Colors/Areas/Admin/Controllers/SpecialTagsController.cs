using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SpecialTagsController : Controller
    {
        private readonly AppDbContext context;

        public SpecialTagsController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.SpecialTags.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SpecialTag tag)
        {
            if (ModelState.IsValid)
            {
                context.SpecialTags.Add(tag);
                await context.SaveChangesAsync();
                TempData["Create"] = "لقد تم إضافة العلامة الخاصة";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }
        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(Id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SpecialTag tag)
        {
            if (ModelState.IsValid)
            {
                context.SpecialTags.Update(tag);
                await context.SaveChangesAsync();
                TempData["Edit"] = "لقد تم تعديل العلامة الخاصة";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }
        [HttpGet]
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(Id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(SpecialTag tag)
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
            var tag = context.SpecialTags.FirstOrDefault(t=>t.Id == Id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, SpecialTag stag)
        {
            if (Id == null)
            {
                return NotFound();
            }
            if (Id != stag.Id)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(Id);
            if (tag == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.SpecialTags.Remove(tag);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف العلامة الخاصة";
                return RedirectToAction(nameof(Index));
            }
            return View(tag);
        }
        #region MyRegion

        #endregion
    }
}
