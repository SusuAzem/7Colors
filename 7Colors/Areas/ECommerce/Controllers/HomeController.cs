using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using _7Colors.Services;
using _7Colors.Data.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using _7Colors.Data.Repository;
using Microsoft.CodeAnalysis;
using _7Colors.ViewModels;

namespace _7Colors.Areas.ECommerce.Controllers
{
    [Area("ECommerce")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeController(ILogger<HomeController> Logger, IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment)
        {
            logger = Logger;
            this.unitOfWork = unitOfWork;
            this.hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> productList =
            unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag");
            ViewData["Types"] = unitOfWork.ProductType.GetAll();            
            return View("Index",productList);
        }
        
        
        [HttpGet]
        public JsonResult Data()
        {
            var products = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag").Select(p =>
            new { id = p.Id, name = p.Name, img = p.Image, type = p.ProductType!.Type, price = p.Price }).ToList();
            return Json(new { data = products });
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var p = unitOfWork.Product.GetFirstOrDefault(u => u.Id == id,
               includeProperties: "ProductType,SpecialTag");
            ShoppingCartLine cartObj = new()
            {
                Count = 1,
                ProductId = id,
                Product = p,
                LinePrice = p.Price
            };
            return View(cartObj);
        }

        [HttpPost]
        [ActionName("Detail")]
        [Authorize]
        [Route("ECommerce/Home/Detail")]
        public ActionResult ProductDetail(ShoppingCartLine shoppingCart)
        {
           
            //products = HttpContext.Session.Get<List<Product>>("products");
            
            var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier);
            shoppingCart.UserNameIdentifier = claim!.Value;

            ShoppingCartLine excart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(
                u => u.UserNameIdentifier == claim.Value && u.ProductId == shoppingCart.ProductId);
            if (excart == null)
            {
                unitOfWork.ShoppingCartLine.Add(shoppingCart);
                unitOfWork.Save();
                HttpContext.Session.SetInt32(StringDefault.SessionCart,
                    unitOfWork.ShoppingCartLine.GetAll(u => u.UserNameIdentifier == claim.Value).ToList().Count);
            }
            else
            {
                unitOfWork.ShoppingCartLine.IncrementCount(excart, shoppingCart.Count);
                unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index), "Home");
        }


        //[ActionName("Remove")]
        //public IActionResult RemoveFromCart(int? id)
        //{
        //    List<Product> products = HttpContext.Session.Get<List<Product>>("products");
        //    if (products != null)
        //    {
        //        var product = products.FirstOrDefault(c => c.Id == id);
        //        if (product != null)
        //        {
        //            products.Remove(product);
        //            HttpContext.Session.Set("products", products);
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        //[HttpPost]
        //public IActionResult Remove(int? id)
        //{
        //    List<Product> products = HttpContext.Session.Get<List<Product>>("products");
        //    if (products != null)
        //    {
        //        var product = products.FirstOrDefault(c => c.Id == id);
        //        if (product != null)
        //        {
        //            products.Remove(product);
        //            HttpContext.Session.Set("products", products);
        //        }
        //    }
        //    return RedirectToAction(nameof(Index));
        //}
        //[HttpGet]
        //public IActionResult Cart()
        //{
        //    List<Product> products = HttpContext.Session.Get<List<Product>>("products");
        //    if (products == null)
        //    {
        //        products = new List<Product>();
        //    }
        //    return View(products);
        //}

        #region API CALLS
        [HttpGet("api/products")]
        public ActionResult GetAll()
        {
            var objFromDb = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag");
            return Json(new { data = objFromDb });
        }
        #endregion
    }
}


