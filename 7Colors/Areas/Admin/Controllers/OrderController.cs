﻿using _7Colors.Data.IRepository;
using _7Colors.Models;
using _7Colors.Services;
using _7Colors.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace _7Colors.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public OrderViewModel? OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderViewModel()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == orderId,
                              includeProperties: "User"),
                OrderItems = _unitOfWork.OrderItem.GetAll(o => o.OrderHeaderId == orderId,
                            includeProperties: "Product")
            };
            return View(OrderVM);
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            var orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "User");
                
            //else
            //{
            //    var claim = User.Identities.FirstOrDefault()!.FindFirst(ClaimTypes.NameIdentifier);
            //    orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.User!.Nameidentifier == claim!.Value, includeProperties: "User");
            //}
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == StringDefault.PaymentStatusPending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == StringDefault.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == StringDefault.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == StringDefault.StatusApproved);
                    break;
                default:
                    break;
            }
            var list = orderHeaders.Select(o => new
            {
                id = o.Id,
                name = o.User!.Name,                
                orderStatus = o.OrderStatus,
                orderTotal = o.OrderTotal
            }).ToList();
            return Json(new { data = list });
        }


        #endregion
    }
}
