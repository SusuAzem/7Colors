using _7Colors.Models;
using _7Colors.ViewModels;
using Microsoft.AspNetCore.Mvc;
using _7Colors.Services;
using _7Colors.Data.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.CodeAnalysis;
using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;

namespace _7Colors.Areas.ECommerce.Controllers
{
    [Area("ECommerce")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly IMapper mapper;
        private readonly INotyfService toastNotification;

        public HomeController(ILogger<HomeController> Logger, IUnitOfWork unitOfWork,
            IWebHostEnvironment hostEnvironment, IMapper mapper, INotyfService toastNotification)
        {
            logger = Logger;
            this.unitOfWork = unitOfWork;
            this.hostEnvironment = hostEnvironment;
            this.mapper = mapper;
            this.toastNotification = toastNotification;
        }
        public IActionResult Index()
        {            
            var vm = new ProductListViewModel()
            {
                Products = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag"),
                Types = unitOfWork.ProductType.GetAll()
            };
            return View(vm);
        }


        [HttpGet]
        public JsonResult Data()
        {
            var products = unitOfWork.Product.GetAll(includeProperties: "ProductType,SpecialTag")
                .Select(p =>
            new { id = p.Id, name = p.Name, img = p.Image, type = p.ProductType!.Type, price = p.Price }).ToList();
            return Json(new { data = products });
        }

        [HttpGet]
        public IActionResult Detail(int id)
        {
            var p = unitOfWork.Product.GetFirstOrDefault(u => u.Id == id,
               includeProperties: "ProductType,SpecialTag");
            ShoppingCartLineViewModel cartObj = new()
            {
                Count = 1,
                ProductId = id,
                LinePrice = p.Price,
                Product = mapper.Map<ProductItemViewModel>(p),
            };
            return View(cartObj);
        }

        [HttpPost]
        [ActionName("Detail")]
        [Authorize]
        [Route("ECommerce/Home/Detail")]
        public async Task<ActionResult> ProductDetail(ShoppingCartLineViewModel shoppingCart)
        {
            var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            shoppingCart.UserNameIdentifier = claim;

            ShoppingCartLine excart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(
                u => u.UserNameIdentifier == claim && u.ProductId == shoppingCart.ProductId);
            if (ModelState.IsValid)
            {
                if (excart == null)
                {
                    excart = new()
                    {
                        UserNameIdentifier = claim,
                        ProductId = shoppingCart.ProductId,
                        Count = 1,
                    };
                    unitOfWork.ShoppingCartLine.Add(excart);
                }
                else
                {
                    unitOfWork.ShoppingCartLine.IncrementCount(excart, shoppingCart.Count);
                }
                HttpContext.Session.SetInt32(StringDefault.SessionCart,
                       unitOfWork.ShoppingCartLine.GetAll(u => u.UserNameIdentifier == claim).ToList().Count);
                await unitOfWork.Save();
                toastNotification.Success("لقد تم إضافة المنتج إلى سلة مشترياتك");
            }
            return RedirectToAction(nameof(Index));
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


