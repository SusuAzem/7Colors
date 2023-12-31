using _7Colors.Data.IRepository;
using _7Colors.Models;
using _7Colors.ViewModels;

using AspNetCoreHero.ToastNotification.Abstractions;

using AutoMapper;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class ImagesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHost;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public ImagesController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost
            ,IMapper mapper, INotyfService toastNotification)
        {
            this.unitOfWork = unitOfWork;
            this.webHost = webHost;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
        }

        public IActionResult Index(int? id = null)
        {
            List<Image> list;
            if (id == null)
            {
                list = unitOfWork.Image.GetAll(null, "Post").ToList();
            }
            else
            {
                list = unitOfWork.Image.GetAll(i => i.PostId == id, "Post").ToList();
            }
            List<ImageViewModel> listViewModel = new();
            foreach (var item in list)
            {
                var i = mapper.Map<ImageViewModel>(item);                 
                listViewModel.Add(i);
            }
            return View(listViewModel);
        }


        [HttpGet]
        public IActionResult Create()
        {
            var vm = new ImageViewModel
            {
                Posts = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title").ToList(),
            };
            //vm.Posts.Insert(0, new SelectListItem("بدون موضوع", "-1"));
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ImageViewModel img, [FromForm] IFormFile Url)
        {
            var existedImg = unitOfWork.Image.GetFirstOrDefault(i => i.Title == img.Title & i.PostId == img.PostId);
            if (existedImg != null)
            {
                ViewBag.message = "هذه الصورة موجودة مسبقاً";
                img.Posts = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title").ToList();
                //img.Posts.Insert(0, new SelectListItem("بدون موضوع", "-1"));
                return View(img);
            }
            if (Url != null)
            {
                var ex = Url.FileName[Url.FileName.LastIndexOf('.')..];
                var fileName = $"img_{DateTime.Now:dd-MM-yy-HH-mm-ss}{ex}";
                var name = Path.Combine(webHost.WebRootPath + "/images/posts/", fileName);
                await Url.CopyToAsync(new FileStream(name, FileMode.Create));
                img.Url = "/images/posts/" + fileName;
            }
            if (Url == null)
            {
                img.Url = "/images/noimage.PNG";
            }
            if (ModelState.IsValid)
            {
                var i = mapper.Map<Image>(img);              
                unitOfWork.Image.Add(i);
                await unitOfWork.Save();
                toastNotification.Success("لقد تم إضافة الصورة");
                return RedirectToAction(nameof(Index));
            }
            img.Posts = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title").ToList();
            //img.Posts.Insert(0, new SelectListItem("بدون موضوع", "-1"));

            return View(img);
        }


        [HttpGet]
        public IActionResult Edit(int? id)
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
            var i = mapper.Map<ImageViewModel>(img);
            i.Posts = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title").ToList();
            //i.Posts.Insert(0, new SelectListItem("بدون موضوع", "-1"));
            return View(i);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ImageViewModel img, IFormFile Url)
        {
            if (Url != null)
            {
                var ex = Url.FileName[Url.FileName.LastIndexOf('.')..];
                var fileName = $"img_{DateTime.Now:dd-MM-yy-HH-mm-ss}{ex}";
                var name = Path.Combine(webHost.WebRootPath + "/images/posts/", fileName);
                await Url.CopyToAsync(new FileStream(name, FileMode.Create));
                img.Url = "/images/posts/" + fileName;
            }
            if (ModelState.IsValid)
            {
                var i = mapper.Map<Image>(img);
                unitOfWork.Image.Update(i);
                await unitOfWork.Save();
                toastNotification.Success("لقد تم تعديل الصورة");
                return RedirectToAction(nameof(Index));
            }
            img.Posts = new SelectList(unitOfWork.Post.GetAll(), "Id", "Title").ToList();
            //img.Posts.Insert(0, new SelectListItem("بدون موضوع", "-1"));
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
            var i = mapper.Map<ImageViewModel>(img);
            return View(i);
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
            var i = mapper.Map<ImageViewModel>(img);
            return View(i);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, ImageViewModel img)
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
                toastNotification.Information("لقد تم حذف الصورة");
                return RedirectToAction(nameof(Index));
            }
            return View(img);
        }    
    }
}
