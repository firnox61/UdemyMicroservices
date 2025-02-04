namespace FreeCourse.Services.Basket.Dtos
{
    public class BasketItemDto
    {

        public int Quantity { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }
    
    public void UpdateBasketItem(int quantity, string courseId, string courseName, decimal price)
        {
            Quantity = quantity;
            CourseId = courseId;
            CourseName = courseName;
            Price = price;
        }

    }
}
