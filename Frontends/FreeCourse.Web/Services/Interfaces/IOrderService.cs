using FreeCourse.Web.Models.Orders;

namespace FreeCourse.Web.Services.Interfaces
{
    public interface IOrderService
    {
        /*
         Senkron iletişim direk olarak order microservisine istek yapacak
        <param name="checkoutInfoInput"</param>*/
        Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput);
        /*
         Asenkron iletişim sipariş bilgilerini rabbit mq ye gönderecek*/
        Task SuspendOrder(CheckoutInfoInput checkoutInfoInput);

        Task<List<OrderViewModel>> GetOrder();
    }
}
