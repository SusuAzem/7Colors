using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MessagesController : Controller
    {
        private readonly AppDbContext context;

        public MessagesController(AppDbContext context)
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
        public IActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(Id);
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
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(Id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ProductType protype)
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
            var type = context.ProductTypes.Find(Id);
            if (type == null)
            {
                return NotFound();
            }
            return View(type);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, ProductType protype)
        {
            if (Id == null)
            {
                return NotFound();
            }
            if (Id != protype.Id)
            {
                return NotFound();
            }
            var type = context.ProductTypes.Find(Id);
            if (type == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.ProductTypes.Remove(protype);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف نوع المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(protype);
        }
        #region MyRegion

        #endregion
    }
}
