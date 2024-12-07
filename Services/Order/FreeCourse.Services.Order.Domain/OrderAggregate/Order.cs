using FreeCourse.Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeCourse.Services.Order.Domain.OrderAggregate
{
    /*
     * ----DDD uygulamakta bize faydalı oldular
     * EF Core features
     * Owned Types
     * Shadow Property
     * Backing Field
     */
    public class Order:Entity,IAggregateRoot
    {
        public DateTime CreatedDate { get; private set; }
        public Address Address { get; private set; }
        public string BuyerId { get; private set; }
        //_orderItems: Siparişe ait ürünleri içeren özel bir liste (bu liste dışarıdan doğrudan erişilemez).
        private readonly List<OrderItem> _orderItems;
        //siparişe ait ürünleri içeren, sadece okunabilir bir koleksiyon. _orderItems listesinin dışarıya açılmış hali.
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;


        public Order()
        {

        }


        public Order(string buyerId, Address address)//herşey bizim kontrolümüzde
        {//Bu kurucu yöntem, yeni bir Order nesnesi oluşturur.
            _orderItems = new List<OrderItem>();//_orderItems listesini başlatır.
            CreatedDate = DateTime.Now;
            BuyerId = buyerId;
            Address = address;
        }
        //order ıtem üretebilmesi için
        public void AddOrderItem(string productId, string productName, decimal price, string pictureUrl)
        {//Bu yöntem, siparişe yeni bir ürün ekler.
            var existProduct=_orderItems.Any(x=>x.ProductId==productId);//listede bu ürün varmı diye kontrol ediyor
            if (!existProduct)
            {
                var newOrderItem=new OrderItem(productId,productName,pictureUrl,price);
                _orderItems.Add(newOrderItem);
            }

        }

        public decimal GetTotalPrice=>_orderItems.Sum(x=>x.Price);

    }
}
