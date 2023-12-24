using _7Colors.Data;
using _7Colors.Data.IRepository;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using System.Reflection.Metadata;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class PostsController : Controller
    {
        private readonly IUnitOfWork  unitOfWork;

        public PostsController(IUnitOfWork context)
        {
            this.unitOfWork = context;
        }
        public IActionResult Index()
        {
            return View(unitOfWork.Post.GetAll(null , "Images"));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(Post post)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Post.Add(post);
                await unitOfWork.Save();
                TempData["Create"] = "لقد تم إضافة موضوع للصفحة الرئيسية";
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
            var post = unitOfWork.Post.GetFirstOrDefault(p=>p.Id == id, "Image");
            if (post == null)
            {
                return NotFound();
            }            
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(Post post)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.Post.Update(post);
                await unitOfWork.Save();
                TempData["Edit"] = "لقد تم تعديل موضوع للصفحة الرئيسية";
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
            var post = unitOfWork.Post.GetFirstOrDefault(p => p.Id == id, "Image"); 
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
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
            return View(post);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int? id, Post post)
        {
            if (id == null)
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
                unitOfWork.Post.Remove(g);
                await unitOfWork.Save();
                TempData["Delete"] = "لقد تم حذف موضوع للصفحة الرئيسية";
                return RedirectToAction(nameof(Index));
            }
            return View(g);
        }
        #region MyRegion

        #endregion
        [HttpPost]
        public async Task<IActionResult> EditPostImgAsync(int[] ids)
        {           
            if (ids == null)
            {
                return BadRequest();
            }
            Image img;
            foreach (var id in ids)
            {
                img= unitOfWork.Image.GetFirstOrDefault(i=>i.Id == id)!;
                img.PostId = 0;
                unitOfWork.Image.Update(img);
            }
            await unitOfWork.Save();
            return Ok();
        }
    }
}
