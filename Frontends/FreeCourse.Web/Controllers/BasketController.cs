﻿using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Models.Discounts;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Web.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly ICatalogService _catalogService;

        public BasketController(IBasketService basketService, ICatalogService catalogService)
        {
            _basketService = basketService;
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _basketService.Get());//varsa gelsin
        }
        public async Task<IActionResult> AddBasketItem(string courseId)
        {
            var course = await _catalogService.GetByCourseId(courseId);
            var basketItem = new BasketItemViewModel
            {
                CourseId = courseId,
                CourseName = course.Name,
                Price = course.Price

            };
            await _basketService.AddBasketItem(basketItem);
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RemoveBasketItem(string courseId)
        {
            await _basketService.RemoveBasketItem(courseId);
            // belirli bir kursu kaldırmak ve ardından sepetin ana sayfasına (Index action'ına) yönlendirme yapmaktır.
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ApplyDiscount(DiscountApplyInput discountApplyInput)
        {
            if (!ModelState.IsValid)
            {
                TempData["discountError"] = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).First();
            }
            var discountStatus = await _basketService.ApplyDiscount(discountApplyInput.Code);
            //uygulandıktan sonra index sayfasına gitsin diye Temp data kullandık requelre arasında data taşıyabilmek için de
            TempData["discountStatus"]=discountStatus;
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> CancelApplyDiscount()
        {
            await _basketService.CancelApplyDiscount();
            return RedirectToAction(nameof(Index));
        }
    }
}
