﻿using FreeCourse.Web.Models.Baskets;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;

namespace FreeCourse.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscountService _discountService;

        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {//api isteği
            _httpClient = httpClient;
            _discountService = discountService;
        }

        public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
        {
            var basket = await Get();
            if (basket != null)
            {
                if(!basket.BasketItems.Any(x=>x.CourseId == basketItemViewModel.CourseId))
                {
                    basket.BasketItems.Add(basketItemViewModel);
                   
                }
                
            }
            else
            {
                basket = new BasketViewModel();
                basket.BasketItems.Add(basketItemViewModel);
            }
           await SaveOrUpdate(basket);
        }

        public async Task<bool> ApplyDiscount(string discountCode)
        {
            await CancelApplyDiscount();
            var basket = await Get();
            if (basket == null )
            {
                return false;
            }
            var hasDiscount=await _discountService.GetDiscount(discountCode);
            if (hasDiscount == null)
            {
                return false;
            }
            basket.ApplyDiscount(hasDiscount.Code,hasDiscount.Rate);
            await SaveOrUpdate(basket);
            return true;
            
        }

        public async Task<bool> CancelApplyDiscount()
        {
           var basket = await Get();
            if(basket==null || basket.DiscountCode==null)
            {
                return false;
            }
            /* basket.DiscountCode = null;
             basket.DiscountRate=null;*/
            basket.CancelDiscont();
             await SaveOrUpdate(basket);
            return true;    
        }

        public async Task<bool> Delete()
        {
            var result = await _httpClient.DeleteAsync("baskets");
            return result.IsSuccessStatusCode;
        }

        public async Task<BasketViewModel> Get()
        {
            var response=await _httpClient.GetAsync("baskets");
            if (!response.IsSuccessStatusCode)
            {//dönüştürme işlemi gerçekleştiriyoruz
               return null;
            }
            var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();
            return basketViewModel.Data;
        }

        public async Task<bool> RemoveBasketItem(string courseId)
        {
            var basket = await Get();
            if (basket == null)
            {
                return false;
            }
            var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);
            if (deleteBasketItem == null)
            {
                return false;
            }
            //burada basketıtemlardan kursu silmeliyiz
            var deleteResult = basket.BasketItems.Remove(deleteBasketItem);
            if(!deleteResult)
            {
                return false;
            }
            if(!basket.BasketItems.Any())
            {
                basket.DiscountCode = null;
            }
            return await SaveOrUpdate(basket);

           
               // var response = await _httpClient.DeleteFromJsonAsync("baskets",co);
        }

        public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync<BasketViewModel>("baskets", basketViewModel);
            return response.IsSuccessStatusCode;
        }
    }
}
