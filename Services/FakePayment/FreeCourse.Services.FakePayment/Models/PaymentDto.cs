﻿namespace FreeCourse.Services.FakePayment.Models
{
    public class PaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public decimal TotalPrice { get; set; }

        //oreadiki tüm propları buraya göndermek için yapıyoruz
        public OrderDto Order { get; set; }
    }
}
