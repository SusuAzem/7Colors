using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class ProductTypesController : Controller
    {
        private readonly AppDbContext context;

        public ProductTypesController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.ProductTypes.ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductType protype)
        {
            if (ModelState.IsValid)
            {
                context.ProductTypes.Add(protype);
                await context.SaveChangesAsync();
                TempData["Create"] = "لقد تم إضافة نوع المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(protype);
        }
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductType protype)
        {
            if (ModelState.IsValid)
            {
                context.ProductTypes.Update(protype);
                await context.SaveChangesAsync();
                TempData["Edit"] = "لقد تم تعديل نوع المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(protype);
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, ProductType protype)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != protype.Id)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(id);
            if (type == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.ProductTypes.Remove(type);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف نوع المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(type);
        }
        #region MyRegion

        #endregion
    }
}
