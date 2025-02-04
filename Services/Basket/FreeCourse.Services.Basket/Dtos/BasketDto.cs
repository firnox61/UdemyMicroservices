namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketDto
    {
        public BasketDto(string userId, string discountCode, int? discountRate)
        {
            UserId = userId;
            DiscountCode = discountCode;
            DiscountRate = discountRate;
        }

       

        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        public int? DiscountRate { get; set; }
        public List<BasketItemDto> basketItems { get; set; }
        public decimal TotalPrice
        {
            get => basketItems.Sum(x => x.Price * x.Quantity);
        }
    }
}
