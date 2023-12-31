using _7Colors.Data.IRepository;
using _7Colors.Models;
using _7Colors.Services;
using _7Colors.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace _7Colors.Areas.ECommerce.Controllers
{
    [Area("ECommerce")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailService service;

        [BindProperty]
        public ShoppingCartViewModel? VM { get; set; }
        public OrderController(IUnitOfWork unitOfWork, IMailService service)
        {
            this.unitOfWork = unitOfWork;
            this.service = service;
        }
        public IActionResult Index()
        {
            var id = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            VM = new ShoppingCartViewModel()
            {
                ListCart = unitOfWork.ShoppingCartLine.GetList(id),
                OrderHeader = new()
            };
            VM.OrderHeader.UserNameIdentifier = id;
            foreach (var cart in VM.ListCart)
            {
                cart.LinePrice = cart.Count * cart.Product!.Price;
                VM.OrderHeader.OrderTotal += cart.LinePrice;
            }
            return View(VM);
        }

        public IActionResult Summary()
        {
            var VM = ShoppingCartOrder();
            return View(VM);
        }

        private ShoppingCartViewModel ShoppingCartOrder()
        {
            var id = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            VM = new ShoppingCartViewModel()
            {
                ListCart = unitOfWork.ShoppingCartLine.GetAll
                (c => c.UserNameIdentifier == id, includeProperties: "Product"),
                OrderHeader = new(),
            };
            VM.OrderHeader.UserNameIdentifier = id;
            VM.OrderHeader.User = unitOfWork.User.GetFirstOrDefault(c => c.NameIdentifier == id);
            foreach (var cartLine in VM.ListCart)
            {
                cartLine.LinePrice = cartLine.Count * cartLine.Product!.Price;
                VM.OrderHeader.OrderTotal += cartLine.LinePrice;
            }
            return VM;
        }
        [HttpPost]
        [Authorize(Policy = "Reg")]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST()
        {
            var VM = ShoppingCartOrder();
            VM!.OrderHeader!.OrderDate = DateTime.Now;
            VM.OrderHeader.PaymentStatus = StringDefault.PaymentStatusPending;
            VM.OrderHeader.OrderStatus = StringDefault.StatusPending;
            
            unitOfWork.OrderHeader.Add(VM.OrderHeader);
            await unitOfWork.Save();

            //VM.OrderHeader.PaymentStatus = StringDefault.PaymentStatusDelayedPayment;
            //VM.OrderHeader.OrderStatus = StringDefault.StatusApproved;
            // transform ShoppingCartLine to OrderItem
            foreach (var cart in VM.ListCart!)
            {
                OrderItem orderItem = new()
                {
                    OrderHeaderId = VM.OrderHeader.Id,
                    ProductId = cart.ProductId,
                    ItemsPrice = cart.LinePrice,
                    Count = cart.Count
                };
                unitOfWork.OrderItem.Add(orderItem);
                await unitOfWork.Save();
            }
            //stripe settings 
            //var domain = "https://localhost:44380";
            //var options = new SessionCreateOptions
            //{
            //    PaymentMethodTypes = new List<string>{ "card"},
            //    LineItems = new List<SessionLineItemOptions>(),
            //    Mode = "payment",
            //    SuccessUrl = domain + $"ECommerce/Order/OrderConfirmation?id={VM.OrderHeader.Id}",
            //    CancelUrl = domain + $"ECommerce/Order/Index",
            //};

            //foreach (var item in VM.ListCart)
            //{
            //    var sessionLineItem = new SessionLineItemOptions
            //    {
            //        PriceData = new SessionLineItemPriceDataOptions
            //        {
            //            UnitAmount = (long)(item.LinePrice),
            //            Currency = "sar",
            //            ProductData = new SessionLineItemPriceDataProductDataOptions
            //            {
            //                Name = item.Product!.Name
            //            },
            //        },
            //        Quantity = item.Count,
            //    };
            //    options.LineItems.Add(sessionLineItem);
            //}

            //var service = new SessionService();
            //Session session = service.Create(options);
            //unitOfWork.OrderHeader.UpdateStripePaymentID(VM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            //await unitOfWork.Save();
            //Response.Headers.Append("Location", session.Url);
            //return new StatusCodeResult(303);
            return RedirectToAction("OrderConfirmation", "Order", new { id = VM.OrderHeader.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "User");
            if (orderHeader.PaymentStatus != StringDefault.PaymentStatusDelayedPayment)
            {
                //var service = new SessionService();
                //Session session = service.Get(orderHeader.SessionId);
                ////check the stripe status
                //if (session.PaymentStatus.ToLower() == "paid")
                //{
                //    unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId!, session.PaymentIntentId);
                unitOfWork.OrderHeader.UpdateStatus(id, StringDefault.StatusApproved, StringDefault.PaymentStatusApproved);
                await unitOfWork.Save();
                //}
            }
            await service.SendMailAsync(new MailData
            {
                ToId = orderHeader.User!.Email!,
                ToName = orderHeader.User.Name,
                Subject = "طلب جديد - الألوان السبعة",
                Body = "\\templates\\NewOrder.html",
                Order = orderHeader,
            });
            var shoppingCarts = unitOfWork.ShoppingCartLine.GetAll(u => u.UserNameIdentifier ==
                orderHeader.UserNameIdentifier).ToList();
            HttpContext.Session.Clear();
            unitOfWork.ShoppingCartLine.RemoveRange(shoppingCarts);
            await unitOfWork.Save();
            return View(id);
        }


        public async Task<IActionResult> Plus(int cartId)
        {
            var cart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(c => c.Id == cartId);
            unitOfWork.ShoppingCartLine.IncrementCount(cart, 1);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(c => c.Id == cartId);

            if (cart.Count <= 1)
            {
                unitOfWork.ShoppingCartLine.Remove(cart);
            }
            else
            {
                unitOfWork.ShoppingCartLine.DecrementCount(cart, 1);
            }
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Remove(int cartId)
        {
            var cart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(c => c.Id == cartId);

            unitOfWork.ShoppingCartLine.Remove(cart);
            await unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
