using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourses.Shared.Dtos;
using FreeCourses.Shared.Messages;
using FreeCourses.Shared.Services;
using MassTransit;
using StackExchange.Redis;

namespace FreeCourse.Services.Basket.Consumer
{
    public class BasketCourseNameChangedEventConsumer : IConsumer<BasketCourseChangeNameEvent>
    {
        
        private readonly IBasketService _basketService;
        

        public BasketCourseNameChangedEventConsumer(IBasketService basketService)
        {
           
            _basketService = basketService;

        }

        public async Task Consume(ConsumeContext<BasketCourseChangeNameEvent> context)
        {
            var newBasket=await _basketService.GetBasket(context.Message.UserId);
            if(newBasket.IsSuccessful)
            {
                newBasket.Data.basketItems.Where(x => x.CourseId == context.Message.CourseId).ToList().ForEach(x =>
                {
                    x.CourseName = context.Message.UpdateName;
                });

                await _basketService.SaveOrUpdate(newBasket.Data);
            }
            //var BasketData=newBasket.Data;
            
           

        }
    }
}
