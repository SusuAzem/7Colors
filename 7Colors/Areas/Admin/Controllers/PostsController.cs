using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using System.Reflection.Metadata;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly AppDbContext context;

        public PostsController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View(context.Posts.Include(g=>g.Images).ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post)
        {
            if (ModelState.IsValid)
            {
                context.Posts.Add(post);
                await context.SaveChangesAsync();
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
            var post = context.Posts.Include(p=>p.Images).FirstOrDefault(p=>p.Id == id);
            if (post == null)
            {
                return NotFound();
            }            
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Post post)
        {
            if (ModelState.IsValid)
            {
                context.Posts.Update(post);
                await context.SaveChangesAsync();
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
            var post = context.Posts.Include(p => p.Images).FirstOrDefault(p => p.Id == id); 
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
            var post = context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id, Post post)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id != post.Id)
            {
                return NotFound();
            }
            var g = context.Posts.Find(id);
            if (g == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                context.Posts.Remove(g);
                await context.SaveChangesAsync();
                TempData["Delete"] = "لقد تم حذف موضوع للصفحة الرئيسية";
                return RedirectToAction(nameof(Index));
            }
            return View(g);
        }
        #region MyRegion

        #endregion
        [HttpPost]
        public async Task<IActionResult> EditPostImg(int[] ids)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);
            if (ids == null)
            {
                return BadRequest();
            }
            Image img;
            foreach (var id in ids)
            {
                img= context.Images.FirstOrDefault(i=>i.Id == id)!;
                img.PostId = 0;
                context.Images.Update(img);
            }
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
