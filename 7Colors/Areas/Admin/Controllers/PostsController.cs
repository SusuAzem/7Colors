using _7Colors.Data;
using _7Colors.Data.IRepository;
using _7Colors.Models;
using _7Colors.Services;
using _7Colors.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class PostsController : Controller
    {
        private readonly IUnitOfWork  unitOfWork;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public PostsController(IUnitOfWork context, IMapper mapper, INotyfService toastNotification)
        {
            this.unitOfWork = context;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {
            var list = unitOfWork.Post.GetAll(includeProperties:"Images").ToList();
            var rp = unitOfWork.Post.GetFirstOrDefault(p => p.Id == -1);
            list.Remove(rp);
            var vml = new List<PostViewModel>();
            foreach (var item in list)
            {
                var p = mapper.Map<PostViewModel>(item);
                vml.Add(p);
            }
            return View(vml);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                var p = mapper.Map<Post>(post);
                unitOfWork.Post.Add(p);
                await unitOfWork.Save();
                toastNotification.Success("لقد تم إضافة موضوع للصفحة الرئيسية");
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = unitOfWork.Post.GetFirstOrDefault(p=>p.Id == id, "Images");
            if (post == null)
            {
                return NotFound();
            }
            var p = mapper.Map<PostViewModel>(post);
            return View(p);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PostViewModel post)
        {
            if (ModelState.IsValid)
            {
                var p = mapper.Map<Post>(post);
                unitOfWork.Post.Update(p);
                await unitOfWork.Save();
                toastNotification.Success("لقد تم تعديل موضوع للصفحة الرئيسية");              
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }
       
        [HttpGet]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = unitOfWork.Post.GetFirstOrDefault(p => p.Id == id, "Images"); 
            if (post == null)
            {
                return NotFound();
            }
            var p = mapper.Map<PostViewModel>(post);
            return View(p);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = unitOfWork.Post.GetFirstOrDefault(p=>p.Id==id);
            if (post == null)
            {
                return NotFound();
            }
            var p = mapper.Map<PostViewModel>(post);
            return View(p);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, PostViewModel post)
        {
            if (id == 0)
            {
                return NotFound();
            }
            if (id != post.Id)
            {
                return NotFound();
            }
            var g = unitOfWork.Post.GetFirstOrDefault(p => p.Id == id);
            if (g == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var imgs = unitOfWork.Image.GetAll(i=>i.PostId == id);
                unitOfWork.Image.RemoveRange(imgs);
                unitOfWork.Post.Remove(g);
                await unitOfWork.Save();
                toastNotification.Information("لقد تم حذف موضوع للصفحة الرئيسية والصور التابعة له");
                return RedirectToAction(nameof(Index));
            }
            return View(g);
        }
        #region MyRegion

        #endregion
        [HttpPost]
        public async Task<IActionResult> EditPostImg(List<int> ids)
        {           
            if (ids == null)
            {
                return BadRequest();
            }
            Image img;
            foreach (var id in ids)
            {
                img= unitOfWork.Image.GetFirstOrDefault(i=>i.Id ==id)!;
                img.PostId = 0;
                unitOfWork.Image.Update(img);
            }
            await unitOfWork.Save();
            return Ok();
        }
    }
}
