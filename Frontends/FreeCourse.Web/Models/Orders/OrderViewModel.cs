namespace FreeCourse.Web.Models.Orders
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        //ÖÇdeme geçmişinde adres alanına ihtiyaç olmadığından dolayı alınmadı
        //public AddressDto Address { get; set; }
        public string BuyerId { get; set; }

        public List<OrderItemvViewModel> OrderItems { get; set; }
    }
}
