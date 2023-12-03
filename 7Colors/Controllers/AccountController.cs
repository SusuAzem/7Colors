using _7Colors.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
using _7Colors.Data;
using _7Colors.Services;
using _7Colors.Data.IRepository;
using _7Colors.ViewModels;

namespace _7Colors.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender sender;
        IMailService mailService;

        public AccountController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, 
            IEmailSender sender, IMailService mailService)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.sender = sender;
            this.mailService = mailService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Account(string nameId)
        {
            var user = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == nameId);
            if (user == null)
            {
                return RedirectToAction(nameof(Login));
            }
            var vm = new UserViewModel()
            {
                Email = user.Email,
                Name = user.Name,
                Nameidentifier = user.NameIdentifier,
                GivenName = user.GivenName,
                Surname = user.Surname,
                Role = user.Role,
                LockoutEnd = user.LockoutEnd,
            };
            if (!user!.Registered)
            {
                vm.Age = 3;
                vm.ParentEmail = "";
                vm.Phone = "05";
                vm.ParentPhone = "05";
                vm.StreetAddress = "";
                vm.City = "";
                vm.Neighborhood = "";
                vm.PostalCode = 10000;
            }
            else
            {
                vm.Age = user.Age;
                vm.ParentEmail = user.ParentEmail;
                vm.Phone = user.Phone;
                vm.ParentPhone = user.ParentPhone;
                vm.StreetAddress = user.StreetAddress;
                vm.City = user.City;
                vm.Neighborhood = user.Neighborhood;
                vm.PostalCode = user.PostalCode;
            }
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Account(UserViewModel user, string nameId)
        {
            var exuser = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == nameId);
            if (exuser == null)
            {
                exuser = await UserHalfReg();
            }
            if (ModelState.IsValid)
            {
                if (exuser.Email == user.Email && exuser.NameIdentifier == user.Nameidentifier)
                {
                    exuser.Registered = true;
                    exuser.StreetAddress = user.StreetAddress!;
                    exuser.City = user.City!;
                    exuser.Neighborhood = user.Neighborhood!;
                    exuser.PostalCode = user.PostalCode;
                    exuser.Age = user.Age;
                    exuser.ParentEmail = user.ParentEmail!;
                    exuser.ParentPhone = user.ParentPhone!;
                    exuser.Phone = user.Phone!;
                    if (!User.HasClaim(claim => claim.Type == "Registered"))
                    {
                        User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "true"));
                    }
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", exuser.Role!));
                    unitOfWork.User.Update(exuser);
                    unitOfWork.Save();
                    //await sender.SendEmailAsync(user.Email!, "الألوان السبعة - تسجيل المستخدم", "لقد تم إكمال تسجيل المعلومات بنجاح");
                    //await sender.SendEmailAsync(user.ParentEmail!, "الألوان السبعة - تسجيل معلومات طفلك", "لقد تم إكمال تسجيل معلومات طفلك بنجاح");
                    await mailService.SendMailAsync(new MailData
                    {
                        ToId = user.Email!,
                        ToName = user.Name!,
                        Subject= "مرحباً بك",
                        Body = "\\templates\\Hello.html",
                    });
                    await mailService.SendMailAsync(new MailData
                    {
                        ToId = user.Email!,
                        ToName = user.Name!,
                        Subject = "مرحباً بك",
                        Body = "\\templates\\RegComG.html",
                    });
                    TempData["Register"] = "لقد تم إكمال تسجيل المعلومات بنجاح";
                }
            }
            User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", exuser!.Role!));
            TempData["NotRegister"] = "لم يتم إكمال تسجيل المعلومات";
            return RedirectToAction(nameof(Index));
        }

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var email = result.Principal!.FindFirstValue(ClaimTypes.Email);
            if (email == StringDefault.AdminEmail)
                return await AdminReg(email);

            var nameId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (nameId == null)
                return BadRequest();
            if (nameId != null)
            {
                var exuser = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == nameId);
                if (exuser == null)
                {
                    exuser = await UserHalfReg();
                }
                if (exuser != null & !exuser!.Registered)
                {
                    return RedirectToAction(nameof(Account), new { nameId });
                }
                if (exuser != null & exuser!.Registered)
                {
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", value: exuser.Role!));
                    return RedirectToAction(nameof(Index), "Home");
                }
            }
            return RedirectToAction(nameof(Index), "Home");
        }

        private async Task<User> UserHalfReg()
        {
            var user = new User(
                    nameidentifier: User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    name: User.FindFirstValue(ClaimTypes.Name)!,
                    givenName: User.FindFirstValue(ClaimTypes.GivenName)!,
                    surname: User.FindFirstValue(ClaimTypes.Surname)!,
                    email: User.FindFirstValue(ClaimTypes.Email)!);
            unitOfWork.User.Add(user);
            await sender.SendEmailAsync(user.Email!, "الألوان السبعة - تسجيل دخول المستخدم", "لقد تم تسجيل دخولك معنا . قم بإكمال إدخال معلوماتك في أقرب وقت");
            unitOfWork.Save();
            TempData["Login"] = "الرجاء إكمال معلومات التسجيل";
            return user;
        }

        private async Task<ActionResult> AdminReg(string? email)
        {
            var admin = unitOfWork.User.GetFirstOrDefault(u => u.Email == email);
            if (admin == null)
            {
                admin = new User(
                    nameidentifier: User.FindFirstValue(ClaimTypes.NameIdentifier)!,
                    name: User.FindFirstValue(ClaimTypes.Name)!,
                    givenName: User.FindFirstValue(ClaimTypes.GivenName)!,
                    surname: User.FindFirstValue(ClaimTypes.Surname)!,
                    email: User.FindFirstValue(ClaimTypes.Email)!)
                {
                    Registered = true,
                    Role = "Admin"
                };
            }
            unitOfWork.User.Add(admin);
            await sender.SendEmailAsync(admin.Email!, "الألوان السبعة - تسجيل دخول المسؤول", " مرحلاً بالأدمن .. لقد تم تسجيل دخولك معنا . قم بإكمال إدخال معلوماتك في أقرب وقت");
            unitOfWork.Save();
            TempData["ARegister"] = "لقد تم إكمال تسجيل معلومات المسؤول بنجاح";
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index), "Home");
        }


        #region validation
        [HttpPost]
        public JsonResult NotEqualEmail(string ParentEmail, string Email)
        {
            var valid = ParentEmail == Email ? false : true;
            return Json(valid);
        }

        [HttpPost]
        public JsonResult NotEqualPhone(string ParentPhone, string Phone)
        {
            var valid = ParentPhone == Phone ? false : true;
            return Json(valid);
        }

        #endregion
    }
}