using _7Colors.Data;
using _7Colors.Data.IRepository;
using _7Colors.Data.Repository;
using _7Colors.Models;
using _7Colors.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHost;     
        
        public ProductsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            this.unitOfWork = unitOfWork;
            webHost = hostEnvironment;
        }
        public IActionResult Index()
        {
            var product = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag");
            return View(product);           
        }
        [HttpGet]
        public IActionResult Create()
        {
            ViewData["TypeId"] = new SelectList(unitOfWork.ProductType.GetAll(), "Id", "Type");
            ViewData["TagId"] = new SelectList(unitOfWork.SpecialTag.GetAll(), "Id", "Name");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product pro, IFormFile Image)
        {
            var types  = new SelectList(unitOfWork.ProductType.GetAll(), "Id", "Type");
            var tags = new SelectList(unitOfWork.SpecialTag.GetAll(), "Id", "Name");
            var existedPro = unitOfWork.Product.GetFirstOrDefault(c => c.Name == pro.Name & c.Price == pro.Price);
            if (existedPro != null)
            {
                ViewBag.message = "هذا المنتج موجود مسبقاً";
                ViewData["TypeId"] = types;
                ViewData["TagId"] = tags;
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
                unitOfWork.Product.Add(pro);
                unitOfWork.Save();
                TempData["Create"] = "لقد تم إضافة المنتج";
                return RedirectToAction(nameof(Index));
            }
            ViewData["TypeId"] = types;
            ViewData["TagId"] = tags;
            return View(pro);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewData["TypeId"] = new SelectList(unitOfWork.ProductType.GetAll(), "Id", "Type");
            ViewData["TagId"] = new SelectList(unitOfWork.SpecialTag.GetAll(), "Id", "Name");
            if (id == null)
            {
                return NotFound();
            }
            var pro = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id , "ProductType, SpecialTag");
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]       
        public async Task<IActionResult> Edit(Product pro, IFormFile Image)
        {          
            if (Image != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images", Path.GetFileName(Image.FileName));
                await Image.CopyToAsync(new FileStream(name, FileMode.Create));
                pro.Image = "/images/" + Image.FileName;              
            }
            if (Image == null)
            {
                pro.Image = "/images/noimage.PNG";
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Update(pro);
                unitOfWork.Save();
                TempData["Edit"] = "لقد تم تعديل المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(pro);
        }
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pro = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id, "ProductType, SpecialTag");
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pro = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id, "ProductType, SpecialTag");
            if (pro == null)
            {
                return NotFound();
            }
            return View(pro);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  IActionResult Delete(int? id, Product pro)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != pro.Id)
            {
                return NotFound();
            }
            var p = unitOfWork.Product.GetFirstOrDefault(c => c.Id == id);
            if (p == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Product.Remove(p);
                unitOfWork.Save();
                TempData["Delete"] = "لقد تم حذف المنتج";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag");
            return Json(new { data = productList });
        }

        //POST
        //[HttpDelete]
        //public IActionResult Delete(int? id)
        //{
        //    var obj = unitOfWork.Product.GetFirstOrDefault(u => u.Id == id);
        //    if (obj == null)
        //    {
        //        return Json(new { success = false, message = "خطأ في عملية الحذف" });
        //    }

        //    var oldImagePath = Path.Combine(hostEnvironment.WebRootPath, obj.Image!.TrimStart('\\'));
        //    if (System.IO.File.Exists(oldImagePath))
        //    {
        //        System.IO.File.Delete(oldImagePath);
        //    }

        //    unitOfWork.Product.Remove(obj);
        //    unitOfWork.Save();
        //    return Json(new { success = true, message = "تم الحذف بنجاح" });

        //}


        #endregion
    }
}

