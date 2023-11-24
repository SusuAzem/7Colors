using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HPGroupsController : Controller
    {
        private readonly AppDbContext context;

        public HPGroupsController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.HPGroups.Include(g=>g.Images).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HPGroup group)
        {
            if (ModelState.IsValid)
            {
                context.HPGroups.Add(group);
                await context.SaveChangesAsync();
                TempData["Create"] = "لقد تم إضافة مجموعة الصفحة الرئيسية";
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }

        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var group = context.HPGroups.Find(Id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HPGroup group)
        {
            if (ModelState.IsValid)
            {
                context.HPGroups.Update(group);
                await context.SaveChangesAsync();
                TempData["Edit"] = "لقد تم تعديل مجموعة الصفحة الرئيسية";
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }
       
        [HttpGet]
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var group = context.HPGroups.Find(Id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(HPGroup group)
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
            var group = context.HPGroups.Find(Id);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, HPGroup group)
        {
            if (Id == null)
            {
                return NotFound();
            }
            if (Id != group.Id)
            {
                return NotFound();
            }
            var g = context.HPGroups.Find(Id);
            if (g == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.HPGroups.Remove(g);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف مجموعة الصفحة الرئيسية";
                return RedirectToAction(nameof(Index));
            }
            return View(g);
        }
        #region MyRegion

        #endregion
    }
}
