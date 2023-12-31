using _7Colors.Data.IRepository;
using _7Colors.Data.Repository;
using _7Colors.Models;
using _7Colors.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static Azure.Core.HttpHeader;

namespace _7Colors.Controllers
{
    [Authorize]
    public class PanelController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailService service;

        public PanelController(IUnitOfWork unitOfWork, IMailService service)
        {
            this.unitOfWork = unitOfWork;
            this.service = service;

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FrontPage()
        {
            return View();
        }
        public IActionResult Orders(string nameId)
        {
            var orderHeaders = unitOfWork.OrderHeader.GetAll(o => o.UserNameIdentifier == nameId);
            var list = orderHeaders.Select(o => new
            {
                id = o.Id,
                name = o.User!.Name,
                orderStatus = o.OrderStatus,
                orderTotal = o.OrderTotal
            }).ToList();
            return View();
        }

    }
}
