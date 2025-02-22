﻿using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        //private readonly IPaymentService _paymentService;
        public async Task<IActionResult> Checkout()
        {//ViewBag, ASP.NET Core MVC'de bir dinamik nesne olarak kullanılan ve bir controller'dan
         //view'a veri aktarmak için kullanılan bir mekanizmadır.
            var basket = await _basketService.Get();
            ViewBag.basket=basket;
            return View( new CheckoutInfoInput());
        }
        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {
            //1. yol senkron iletişim
            //var orderStatus = await _orderService.CreateOrder(checkoutInfoInput);
            //2. yol asenkron iletişim
            var orderSuspend = await _orderService.SuspendOrder(checkoutInfoInput);

            if (!orderSuspend.IsSuccessful)
            {
                /*TempData["error"]=orderStatus.Error;başarılı değilse tekrar Checkout() a göndermek istiyorum
                return RedirectToAction(nameof(Checkout));*/
                var basket = await _basketService.Get();
                ViewBag.basket = basket;
                ViewBag.error= orderSuspend.Error;
                return View();
            }//başarılıysa SuccessfulCheckout sayfasıına yönelndirilsin

            //1. yol senkron iletişim
           // return RedirectToAction(nameof(SuccessfulCheckout), new { orderId = orderSuspend.OrderId });

            //2. yol asenkron iletişim
            return RedirectToAction(nameof(SuccessfulCheckout),new {orderId= new Random().Next(1,1000)});

        }
        public IActionResult SuccessfulCheckout(int orderId)
        {
            ViewBag.orderId = orderId;
            return View();
        }
        public async Task<IActionResult> CheckoutHistory()
        {
            return View(await _orderService.GetOrder());
        }
    }
}
