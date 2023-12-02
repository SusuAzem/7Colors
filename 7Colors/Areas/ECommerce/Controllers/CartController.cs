using _7Colors.Data.IRepository;
using _7Colors.Models;
using _7Colors.Services;
using _7Colors.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Stripe.Checkout;

using System.Security.Claims;

namespace _7Colors.Areas.ECommerce.Controllers
{
    [Area("ECommerce")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IEmailSender emailSender;

        [BindProperty]
        public ShoppingCartViewModel? VM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            this.unitOfWork = unitOfWork;
            this.emailSender = emailSender;
        }
        public IActionResult Index()
        {
            var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier);

            VM = new ShoppingCartViewModel()
            {
                ListCart = unitOfWork.ShoppingCartLine.GetAll(u =>
                u.UserNameIdentifier == claim!.Value, includeProperties: "Product"),
                OrderHeader = new(claim!.Value),
            };
            foreach (var cart in VM.ListCart)
            {
                cart.LinePrice = cart.Count * cart.Product!.Price;
                VM.OrderHeader.OrderTotal += cart.LinePrice;
            }
            return View(VM);
        }

        public IActionResult Summary()
        {
            var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier);
            VM = new ShoppingCartViewModel()
            {
                ListCart = unitOfWork.ShoppingCartLine.GetAll
                (c => c.UserNameIdentifier == claim!.Value, includeProperties: "Product"),
                OrderHeader = new(claim!.Value),
            };
            var user = unitOfWork.User
                .GetFirstOrDefault(c => c.NameIdentifier == claim!.Value);
            VM.OrderHeader.User = user;
            VM.OrderHeader.UserNameIdentifier = user.NameIdentifier;            

            foreach (var cartLine in VM.ListCart)
            {
                cartLine.LinePrice = cartLine.Count * cartLine.Product!.Price;
                VM.OrderHeader.OrderTotal += cartLine.LinePrice;
            }
            return View(VM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPOST()
        {
            var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier);
            User user = unitOfWork.User.GetFirstOrDefault(u => u.NameIdentifier == claim!.Value);
            VM!.ListCart = unitOfWork.ShoppingCartLine.GetAll
                (u => u.UserNameIdentifier == claim!.Value,includeProperties: "Product");
            VM!.OrderHeader!.OrderDate = DateTime.Now;
            VM!.OrderHeader.UserNameIdentifier = user.NameIdentifier;
            VM.OrderHeader.User = user;
            VM.OrderHeader.PaymentStatus = StringDefault.PaymentStatusPending;
            VM.OrderHeader.OrderStatus = StringDefault.StatusPending;
            foreach (var cartLine in VM.ListCart)
            {
                cartLine.LinePrice = cartLine.Count * cartLine.Product!.Price;
                VM.OrderHeader.OrderTotal += cartLine.LinePrice;
            }
            unitOfWork.OrderHeader.Add(VM.OrderHeader);
            unitOfWork.Save();

            //VM.OrderHeader.PaymentStatus = StringDefault.PaymentStatusDelayedPayment;
            //VM.OrderHeader.OrderStatus = StringDefault.StatusApproved;
            // transform ShoppingCartLine to OrderItem
            foreach (var cart in VM.ListCart)
            {
                OrderItem orderItem = new()
                {
                    OrderHeaderId = VM.OrderHeader.Id,
                    ProductId = cart.ProductId,
                    ItemsPrice = cart.LinePrice,
                    Count = cart.Count
                };
                unitOfWork.OrderItem.Add(orderItem);
                unitOfWork.Save();
            }           
                //stripe settings 
                var domain = "https://localhost:44349/";
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                {
                  "card",
                },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"ECommerce/Cart/OrderConfirmation?id={VM.OrderHeader.Id}",
                    CancelUrl = domain + $"ECommerce/Cart/Index",
                };

                foreach (var item in VM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.LinePrice),
                            Currency = "sar",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product!.Name
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);
                unitOfWork.OrderHeader.UpdateStripePaymentID(VM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
            //return RedirectToAction("OrderConfirmation", "Cart", new { id = VM.OrderHeader.Id }, new StatusCodeResult(303).ToString());
            }
        
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id, includeProperties: "User");
            if (orderHeader.PaymentStatus != StringDefault.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                //check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    unitOfWork.OrderHeader.UpdateStripePaymentID(id, orderHeader.SessionId!, session.PaymentIntentId);
                    unitOfWork.OrderHeader.UpdateStatus(id, StringDefault.StatusApproved, StringDefault.PaymentStatusApproved);
                    unitOfWork.Save();
                }
            }
            emailSender.SendEmailAsync(orderHeader.User!.Email!, "طلب جديد - الألوان السبعة", "<p>تم إنشاء طلب جديد</p>");
            List<ShoppingCartLine> shoppingCarts = unitOfWork.ShoppingCartLine.GetAll(u => u.UserNameIdentifier ==
            orderHeader.UserNameIdentifier).ToList();
            HttpContext.Session.Clear();
            unitOfWork.ShoppingCartLine.RemoveRange(shoppingCarts);
            unitOfWork.Save();
            return View(id);
        }


        public IActionResult Plus(int cartId)
        {
            var cart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(c => c.Id == cartId);
            unitOfWork.ShoppingCartLine.IncrementCount(cart, 1);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
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
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = unitOfWork.ShoppingCartLine.GetFirstOrDefault(c => c.Id == cartId);

            unitOfWork.ShoppingCartLine.Remove(cart);
            unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
