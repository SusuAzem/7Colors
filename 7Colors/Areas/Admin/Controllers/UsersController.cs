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
        public IActionResult GetAll(string? filter)
        {
            IEnumerable userList;
            if (filter!= null)
            {
                    userList = context.Users.Where(u => u.Role == filter).Select(
                   u => new { name = u.Name, email = u.Email, phone = u.Phone, role = u.Role }).ToList();
            }
            else
            {
                    userList = context.Users.Select(
                    u => new { name = u.Name, email = u.Email, phone = u.Phone, role = u.Role , id = u.Nameidentifier }).ToList();
            }                   
            return Json(new { data = userList });
        }
        public async Task<IActionResult> Upgrade(string id)
        {
            var teacher = context.Users.FirstOrDefault(u => u.Nameidentifier == id);
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
            var teacher = context.Users.FirstOrDefault(u => u.Nameidentifier == id);
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
            var teacher = context.Users.FirstOrDefault(u => u.Nameidentifier == id);
            if (teacher != null)
            {
                context.Users.Remove(teacher);
                await context.SaveChangesAsync();
                TempData["Block"] = $"لقد تم حذف المستخدم {teacher.Name}";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
