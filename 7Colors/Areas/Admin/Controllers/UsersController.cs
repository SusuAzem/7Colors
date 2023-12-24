using _7Colors.Data.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public UsersController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAll()
        {
            var userList = unitOfWork.User.GetAll().Select(
                    u => new { name = u.Name, email = u.Email, phone = u.Phone, role = u.Role , 
                        id = u.NameIdentifier, lockoutEnd = u.LockoutEnd }).ToList();                            
            return Json(new { data = userList });
        }

        public async Task<IActionResult> Upgrade(string id)
        {
            var teacher = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                teacher.Role = "Teacher";
                unitOfWork.User.Update(teacher);
                await unitOfWork.Save();
                TempData["Upgrade"] = $"لقد تم تعيين المستخدم {teacher.Name} كمعلم";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Downgrade(string id)
        {
            var teacher = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                teacher.Role = "Student";
                unitOfWork.User.Update(teacher);
                await unitOfWork.Save();
                TempData["Downgrade"] = $"لقد تم التعيين المستخدم {teacher.Name} كطالب";
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Block(string id)
        {
            var teacher = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == id);
            if (teacher != null)
            {
                unitOfWork.User.Remove(teacher);
                await unitOfWork.Save();
                TempData["Block"] = $"لقد تم حذف المستخدم {teacher.Name}";
                return RedirectToAction(nameof(Index));
            }
            TempData["Block"] = "خطأ خلال عملية الحذف";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> LockUnlock(string id)
        {
            var exuser = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == id);
            if (exuser == null)
            {
                TempData["Lock"] = "خطأ خلال عملية الحجب";
                return RedirectToAction(nameof(Index));
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
            unitOfWork.User.Update(exuser);
            await unitOfWork.Save();
            TempData["Lock"] = "تمت العملية بنجاح";
            return RedirectToAction(nameof(Index));
        }
    }
}
