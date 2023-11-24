using _7Colors.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using _7Colors.Data;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using _7Colors.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace _7Colors.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext context;
        private readonly IEmailSender sender;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IEmailSender sender)
        {
            _logger = logger;
            this.context = context;
            this.sender = sender;
        }

        public IActionResult Index()
        {
            return View(context.HPGroups.Include(g => g.Images).ToList());
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        [HttpGet]
        [Authorize]
        public IActionResult Account(string nameId)
        {
            var user = context.Users.FirstOrDefault(u => u.Nameidentifier == nameId);
            if (user == null) { return BadRequest(); }
            if (! user!.Registered)
            {
                user.Age = 3;
                user.ParentEmail = "";
                user.Phone = "05";
                user.ParentPhone = "05";
                user.StreetAddress = "";
                user.City = "";
                user.Neighborhood = "";
                user.PostalCode = 10000;
                return View(user);
            }
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Account(User user, string nameId)
        {
            if (ModelState.IsValid)
            {
                var exuser = context.Users.FirstOrDefault(u => u.Nameidentifier == nameId);

                if (exuser != null && exuser.Email == user.Email)
                {
                    user.Registered = true;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    sender.SendEmail(user.Email!, "الألوان السبعة - تسجيل المستخدم", "لقد تم إكمال تسجيل المعلومات بنجاح");
                    sender.SendEmail(user.ParentEmail!, "الألوان السبعة - تسجيل معلومات طفلك", "لقد تم إكمال تسجيل معلومات طفلك بنجاح");
                    TempData["Register"] = "لقد تم إكمال تسجيل المعلومات بنجاح";
                }
            }
            TempData["NotRegister"] = "لم يتم إكمال تسجيل المعلومات";
            return RedirectToAction(nameof(Index));
        }
        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
                //RedirectUri = "/"
            });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // admin checking and saving
            var email = result.Principal!.FindFirstValue(ClaimTypes.Email);
            var admin = context.Users.FirstOrDefault(u => u.Email == email);
            if (email == "soso.g.f.86@gmail.com" & admin == null)
            {
                admin = new User
                {
                    Email = email,
                    Nameidentifier = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier),
                    Name = User.FindFirstValue(ClaimTypes.Name),
                    Registered = true,
                    Role = "Admin"
                };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
                TempData["Register"] = "لقد تم إكمال تسجيل المعلومات بنجاح";
                return RedirectToAction(nameof(Index));
            }
            // user checking and saving
            var nameId = result.Principal!.FindFirstValue(ClaimTypes.NameIdentifier);
            if (nameId != null)
            {
                var exuser = context.Users.FirstOrDefault(u => u.Nameidentifier == nameId);
                if (exuser == null)
                {
                    var user = new User
                    {
                        Nameidentifier = User.FindFirstValue(ClaimTypes.NameIdentifier),
                        Name = User.FindFirstValue(ClaimTypes.Name),
                        GivenName = User.FindFirstValue(ClaimTypes.GivenName),
                        Surname = User.FindFirstValue(ClaimTypes.Surname),
                        Email = User.FindFirstValue(ClaimTypes.Email),
                        Age = 3,
                        ParentEmail = "-",
                        Phone = "05",
                        ParentPhone = "05",
                        StreetAddress = "-",
                        City = "-",
                        Neighborhood = "-",
                        PostalCode = 10000,
                        Role = "Student",
                        Registered = false
                    };
                    context.Users.Add(user);
                    sender.SendEmail(user.Email!, "الألوان السبعة - تسجيل دخول المستخدم", "لقد تم تسجيل دخولك معنا . قم بإكمال إدخال معلوماتك في أقرب وقت");
                    await context.SaveChangesAsync();
                    TempData["Login"] = "الرجاء إكمال معلومات التسجيل";
                }
                if (exuser != null & ! exuser!.Registered)
                {
                    return RedirectToAction(nameof(Account), new { nameId });
                }
            }
            else
            {
                return BadRequest();
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public JsonResult NotEqualEmail(string ParentEmail, string  Email)
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
    }
}