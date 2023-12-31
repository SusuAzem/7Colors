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
using AutoMapper;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace _7Colors.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailService mailService;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public AccountController(ILogger<HomeController> logger, IUnitOfWork unitOfWork,
             IMailService mailService, IMapper mapper, INotyfService toastNotification)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
            this.mailService = mailService;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
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
            
            var vm = mapper.Map<UserViewModel>(user);           
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
            exuser ??= await UserHalfReg();

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

                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "true"));
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", exuser.Role!));
                    unitOfWork.User.Update(exuser);
                    await unitOfWork.Save();
                    await mailService.SendMailAsync(new MailData
                    {
                        ToId = user.Email!,
                        ToName = user.Name!,
                        Subject = "مرحباً بك",
                        Body = "\\templates\\RegCom.html",
                    });
                    await mailService.SendMailAsync(new MailData
                    {
                        ToId = user.Email!,
                        ToName = user.Name!,
                        Subject = "مرحباً بك",
                        Body = "\\templates\\RegComG.html",
                    });
                    toastNotification.Success("لقد تم إكمال تسجيل المعلومات بنجاح");
                }
            }
            User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", exuser!.Role!));
            User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "false"));
            toastNotification.Warning("لم يتم إكمال تسجيل المعلومات");
            return RedirectToAction(nameof(Index), "Home");
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
            if (email == StringDefault.AdminEmail1 || email == StringDefault.AdminEmail2)
                return await AdminReg(email);

            var nameId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (nameId == null)
                return BadRequest();
            if (nameId != null)
            {
                var exuser = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == nameId);
                exuser ??= await UserHalfReg();
                if (exuser.LockoutEnd > DateTimeOffset.Now)
                {
                    await HttpContext.SignOutAsync();
                    toastNotification.Custom("قد تم حجب المستخدم عن تسجيل الدخول للمنصة.. تواصل مع الإدارة لرفع الحجب", 10, "red");
                    return RedirectToAction(nameof(Index), "Home");
                }
                if (exuser != null & !exuser!.Registered)
                {
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "false"));
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", value: exuser.Role!));
                    toastNotification.Warning("الرجاء القيام بإكمال معلومات المستخدم");
                    return RedirectToAction(nameof(Account), new { nameId });
                }
                if (exuser != null & exuser!.Registered)
                {
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "true"));
                    User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", value: exuser.Role!));
                    toastNotification.Success("لقد تم تسجيل الدخول");
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
            await mailService.SendMailAsync(new MailData
            {
                ToId = user.Email!,
                ToName = user.Name!,
                Subject = "مرحباً بك",
                Body = "\\templates\\Hello.html",
            });
            await unitOfWork.Save();
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

                unitOfWork.User.Add(admin);
                User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "true"));
                User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", admin.Role));
                await mailService.SendMailAsync(new MailData
                {
                    ToId = admin.Email!,
                    ToName = admin.Name!,
                    Subject = "مرحباً بالأدمن",
                    Body = "\\templates\\HelloAdmin.html",
                });
                await unitOfWork.Save();
                toastNotification.Success("لقد تم إكمال تسجيل معلومات المسؤول بنجاح");
                return RedirectToAction(nameof(Index), "Home");
            }
            User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Registered", "true"));
            User.Identities.FirstOrDefault()!.AddClaim(new Claim(type: "Role", admin.Role));
            return RedirectToAction(nameof(Index), "Home");
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
            var valid = ParentEmail != Email;
            return Json(valid);
        }

        [HttpPost]
        public JsonResult NotEqualPhone(string ParentPhone, string Phone)
        {
            var valid = ParentPhone != Phone;
            return Json(valid);
        }

        #endregion
    }
}