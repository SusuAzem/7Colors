using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class UsersController : Controller
    {
        private readonly AppDbContext context;

        public UsersController(AppDbContext context)
        {
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = context.Users.Select(
                    u => new { name = u.Name, email = u.Email, phone = u.Phone, role = u.Role , id = u.NameIdentifier }).ToList();                            
            return Json(new { data = userList });
        }
        public async Task<IActionResult> Upgrade(string id)
        {
            var teacher = context.Users.FirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                teacher.Role = "Teacher";
                context.Users.Update(teacher);
                await context.SaveChangesAsync();
                TempData["Upgrade"] = $"لقد تم تعيين المستخدم {teacher.Name} كمعلم";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Downgrade(string id)
        {
            var teacher = context.Users.FirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                teacher.Role = "Student";
                context.Users.Update(teacher);
                await context.SaveChangesAsync();
                TempData["Downgrade"] = $"لقد تم التعيين المستخدم {teacher.Name} كطالب";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Block(string id)
        {
            var teacher = context.Users.FirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                context.Users.Remove(teacher);
                await context.SaveChangesAsync();
                TempData["Block"] = $"لقد تم حذف المستخدم {teacher.Name}";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
      
        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            var exuser = context.Users.FirstOrDefault(u => u.NameIdentifier == id);
            if (exuser == null)
            {
                return Json(new { success = false, message = "خطأ خلال عملية الحجب" });
            }

            if  (exuser.LockoutEnd > DateTime.Now)
            {
                //user is currently locked, we will unlock them
                exuser.LockoutEnd = DateTime.Now;
            }
            else
            {
                exuser.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            context.Users.Update(exuser);
            await context.SaveChangesAsync();
            return Json(new { success = true, message = "تمت العملية بنجاح" });
        }
    }
}
