using _7Colors.Data;
using _7Colors.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json;
using _7Colors.Services;

namespace _7Colors.Areas.ECommerce.Controllers
{
    [Area("ECommerce")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly AppDbContext context;

        public HomeController(ILogger<HomeController> Logger, AppDbContext context)
        {
            logger = Logger;
            this.context = context;
        }
        public IActionResult Index()
        {
            //return new JsonResult(context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).ToList(), 
            //    serializerSettings: new JsonSerializerSettings() );
            return View(context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).ToList());
        }
        [HttpGet]
        public JsonResult Data()
        {
            var products = context.Products.Include(c => c.ProductType).Include(c => c.SpecialTag).Select(p=>
            new {id= p.Id, name = p.Name,img = p.Image, type=p.ProductType!.Type, price = p.Price}).ToList();
            return Json(new {data = products});
        }
        [HttpGet]
        public IActionResult Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = context.Products.Include(c => c.ProductType).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult AddToCart(int? id)
        {
            List<Product> products = new List<Product>();
            if (id == null)
            {
                return NotFound();
            }

            var product = context.Products.Include(c => c.ProductType).FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null)
            {
                products = new List<Product>();
            }
            products.Add(product);
            HttpContext.Session.Set("products", products);
            return RedirectToAction(nameof(Index));

        }

        [ActionName("Remove")]
        public IActionResult RemoveFromCart(int? id)
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);

                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int? id)
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products != null)
            {
                var product = products.FirstOrDefault(c => c.Id == id);
                if (product != null)
                {
                    products.Remove(product);
                    HttpContext.Session.Set("products", products);

                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Cart()
        {
            List<Product> products = HttpContext.Session.Get<List<Product>>("products");
            if (products == null)
            {
                products = new List<Product>();
            }
            return View(products);

        }

    }
}

