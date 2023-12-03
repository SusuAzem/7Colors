using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
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
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(id);
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
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }


        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.FirstOrDefault(t=>t.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, SpecialTag stag)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != stag.Id)
            {
                return NotFound();
            }
            var tag = context.SpecialTags.Find(id);
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
