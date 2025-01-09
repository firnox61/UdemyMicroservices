using FreeCourse.Web.Models.FakePayments;
using FreeCourse.Web.Models.Orders;
using FreeCourse.Web.Services.Interfaces;
using FreeCourses.Shared.Dtos;
using FreeCourses.Shared.Services;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly IBasketService _basketService;
        private readonly IPaymentService _paymentService;
        private readonly ISharedIdentityService _sharedIdentityService;

        public OrderService(HttpClient httpClient, IBasketService basketService, IPaymentService paymentService
            , ISharedIdentityService sharedIdentityService)
        {
            _httpClient = httpClient;
            _basketService = basketService;
            _paymentService = paymentService;
            _sharedIdentityService=sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();
            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = checkoutInfoInput.TotalPrice,
            };
            var responsePayment=await _paymentService.ReceivePayment(paymentInfoInput);
            if(!responsePayment)
            {
                return new OrderCreatedViewModel()
                {
                    Error = "Ödeme alınamadı",
                    IsSuccessful = false,
                };
            }
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Street = checkoutInfoInput.Street,
                    ZipCode = checkoutInfoInput.ZipCode,
                    Line = checkoutInfoInput.Line,

                },
                

            };
            basket.BasketItems.ForEach (x =>
            {
                var orderItem = new OrderItemCreateInput
                {
                    ProductId = x.CourseId,
                    ProductName = x.CourseName,
                    Price = x.Price,
                    PictureUrl = ""
                };
                orderCreateInput.OrderItems.Add(orderItem);
            });
            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("orders", orderCreateInput);
            if(!response.IsSuccessStatusCode)
            {

                return new OrderCreatedViewModel()
                {
                    Error = "Sipariş oluşturulamadı",
                    IsSuccessful = false,
                };
            }
            return await response.Content.ReadFromJsonAsync<OrderCreatedViewModel>();
            


            }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("orders");
            return response.Data;
        }

        public Task SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            throw new NotImplementedException();
        }
    }
}
