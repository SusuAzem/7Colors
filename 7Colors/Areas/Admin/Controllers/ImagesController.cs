using _7Colors.Data;
using _7Colors.Data.IRepository;
using _7Colors.Data.Repository;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy ="Admin")]
    public class ImagesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHost;

        public ImagesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            this.unitOfWork = unitOfWork;
            this.webHost = webHost;
        }

        public IActionResult Index(int? id = null)
        {
            if (id == null)
            {
                return View(unitOfWork.Image.GetAll(null ,"Post"));
            }
            else
            {
                return View(unitOfWork.Image.GetAll(i=>i.PostId == id, "Post"));
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewData["PostId"] = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Image img, [FromForm] IFormFile Url)
        {
            var existedImg = unitOfWork.Image.GetFirstOrDefault(i => i.Title == img.Title & i.PostId == img.PostId);
            if (existedImg != null)
            {
                ViewBag.message = "هذه الصورة موجودة مسبقاً";
                ViewData["PostId"] = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title");
                return View(img);
            }
            if (Url != null)
            {
                var name = Path.Combine(webHost.WebRootPath + "/images/posts/", Path.GetFileName(Url.FileName));
                await Url.CopyToAsync(new FileStream(name, FileMode.Create));
                img.Url = "/images/posts/" + Url.FileName;
            }
            if (Url == null)
            {
                img.Url = "/images/noimage.PNG";
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Image.Add(img);
                await unitOfWork.Save();
                TempData["Create"] = "لقد تم إضافة الصورة";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostId"] = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title");
            return View(img);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            ViewData["PostId"] = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title");
            if (id == null)
            {
                return NotFound();
            }
            var img = unitOfWork.Image.GetFirstOrDefault(c => c.Id == id, "Post");
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
                var name = Path.Combine(webHost.WebRootPath + "/images/posts/", Path.GetFileName(Url.FileName));
                await Url.CopyToAsync(new FileStream(name, FileMode.Create));
                img.Url = "/images/posts/" + Url.FileName;
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Image.Update(img);
                await unitOfWork.Save();
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
            var img = unitOfWork.Image.GetFirstOrDefault(c => c.Id == id, "Post");
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
            var img = unitOfWork.Image.GetFirstOrDefault(c => c.Id == id, "Post");
            if (img == null)
            {
                return NotFound();
            }
            return View(img);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int? id, Image img)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != img.Id)
            {
                return NotFound();
            }
            var p = unitOfWork.Image.GetFirstOrDefault(c => c.Id == id);
            if (p == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                unitOfWork.Image.Remove(p);
                await unitOfWork.Save();
                TempData["Delete"] = "لقد تم حذف الصورة";
                return RedirectToAction(nameof(Index));
            }
            return View(p);
        }
        #region MyRegion

        #endregion
    }
}
