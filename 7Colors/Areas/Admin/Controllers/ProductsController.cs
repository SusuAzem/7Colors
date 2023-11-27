using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext context;
        private readonly IWebHostEnvironment webHost;
        
        public ProductsController(AppDbContext context, IWebHostEnvironment webHost)
        {
            this.context = context;
            this.webHost = webHost;
        }

        public IActionResult Index()
        {
            return View(context.Products.Include(p => p.ProductType).Include(p => p.SpecialTag).ToList());
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "Type");
            ViewData["TagId"] = new SelectList(context.SpecialTags.ToList(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product pro, IFormFile Image)
        {
            var existedPro = context.Products.FirstOrDefault(c => c.Name == pro.Name & c.Price == pro.Price);
            if (existedPro != null)
            {
                ViewBag.message = "هذا المنتج موجود مسبقاً";
                ViewData["TypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "Type");
                ViewData["TagId"] = new SelectList(context.SpecialTags.ToList(), "Id", "Name");
                return View(pro);
            }
            if (Image != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images", Path.GetFileName(Image.FileName));
                Image.CopyTo(new FileStream(name, FileMode.Create));
                pro.Image = "/images/" + Image.FileName;                
            }
            if (Image == null)
            {
                pro.Image = "/images/noimage.PNG";
            }
            if (ModelState.IsValid)
            {
                context.Products.Add(pro);
                await context.SaveChangesAsync();
                TempData["Create"] = "لقد تم إضافة المنتج";
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "Type");
            ViewData["TagId"] = new SelectList(context.SpecialTags.ToList(), "Id", "Name");
            return View(pro);
        }


        [HttpGet]
        public IActionResult Edit(int? Id)
        {
            ViewData["TypeId"] = new SelectList(context.ProductTypes.ToList(), "Id", "Type");
            ViewData["TagId"] = new SelectList(context.SpecialTags.ToList(), "Id", "Name");
            if (Id == null)
            {
                return NotFound();
            }
            var pro = context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).FirstOrDefault(c => c.Id == Id);
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> Edit(Product pro, IFormFile image)
        {          
            if (image != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images", Path.GetFileName(image.FileName));
                await image.CopyToAsync(new FileStream(name, FileMode.Create));
                pro.Image = "/images/" + image.FileName;              
            }            
            if (ModelState.IsValid)
            {
                context.Products.Update(pro);
                await context.SaveChangesAsync();
                TempData["Edit"] = "لقد تم تعديل المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(pro);
        }
        [HttpGet]
        public IActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            var pro = context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).FirstOrDefault(c => c.Id == Id);
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(Product pro)
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
            var pro = context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).FirstOrDefault(c => c.Id == Id);
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? Id, Product pro)
        {
            if (Id == null)
            {
                return NotFound();
            }
            if (Id != pro.Id)
            {
                return NotFound();
            }
            var p = context.Products.FirstOrDefault(c => c.Id == Id);
            if (p == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.Products.Remove(p);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        #region MyRegion

        #endregion
    }
}
