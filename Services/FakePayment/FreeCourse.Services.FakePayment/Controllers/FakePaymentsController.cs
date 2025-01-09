using FreeCourse.Services.FakePayment.Models;
using FreeCourses.Shared.ControllerBases;
using FreeCourses.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FreeCourse.Services.FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentsController : CustomBaseController
    {
        [HttpPost]
        public IActionResult RecievePayment(PaymentDto paymentDto)
        {
            //paymentdto ile ödeme gerçekleştir
            return CreateActionResultInstance(Response<NoContent>.Success(200));
        }
    }
}
