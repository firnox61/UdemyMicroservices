using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using FreeCourse.Web.Services;

namespace FreeCourse.Web.Extensions
{
    public static class ServiceExtension
    {
        public static void AddHttpClientServices(this IServiceCollection services, IConfiguration Configuration)
        {
            var serviceApiSettings = Configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();


            services.AddHttpClient<IClientCredentialTokenService, ClientCredentialTokenService>();
            services.AddHttpClient<IIdentityService, IdentityService>();
            services.AddHttpClient<ICatalogService, CatalogService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Catalog.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();//token varse ekleyecek yoksa yenisini alıp ekleyecek


            services.AddHttpClient<IPhotoStockService, PhotoStockService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.PhotoStock.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();//token varse ekleyecek yoksa yenisini alıp ekleyecek



            //Userservice içnde herhangi bir client kullanıldığında  git bu hanlerı çalıştır diyoıruz
            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUrl);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();


            services.AddHttpClient<IBasketService, BasketService>(opt =>
            {

                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Basket.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IDiscountService, DiscountService>(opt =>
            {

                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Discount.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            services.AddHttpClient<IPaymentService, PaymentService>(opt =>
            {

                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Payment.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();
            services.AddHttpClient<IOrderService, OrderService>(opt =>
            {

                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUrl}/{serviceApiSettings.Order.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();



        }
    }
}
