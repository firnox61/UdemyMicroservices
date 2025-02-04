using FreeCourse.Services.FakePayment.Models;
using FreeCourses.Shared.ControllerBases;
using FreeCourses.Shared.Dtos;
using FreeCourses.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public FakePaymentsController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> RecievePaymentAsync(PaymentDto paymentDto)
        {
            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));
            //paymentdto ile ödeme gerçekleştir
            var createOrderMessageCommand = new CreateOrderMessageCommand();
            createOrderMessageCommand.BuyerId=paymentDto.Order.BuyerId;
            createOrderMessageCommand.Province = paymentDto.Order.Address.Province;
            createOrderMessageCommand.District=paymentDto.Order.Address.Street;
            createOrderMessageCommand.Street= paymentDto.Order.Address.Street;
            createOrderMessageCommand.Line= paymentDto.Order.Address.Line;
            createOrderMessageCommand.ZipCode=paymentDto.Order.Address.ZipCode;

            paymentDto.Order.OrderItems.ForEach(x =>
            {
                createOrderMessageCommand.OrderItems.Add(new OrderItem
                {
                    ProductName = x.ProductName,
                    ProductId = x.ProductId,
                    PictureUrl = x.PictureUrl,
                    Price = x.Price,
                });
            });

            await sendEndpoint.Send<CreateOrderMessageCommand>(createOrderMessageCommand);

            return CreateActionResultInstance(FreeCourses.Shared.Dtos.Response<NoContent>.Success(200));
        }
    }
}
