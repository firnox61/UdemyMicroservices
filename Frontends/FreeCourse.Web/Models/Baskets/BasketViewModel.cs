namespace FreeCourse.Web.Models.Baskets
{
    public class BasketViewModel
    {
        public BasketViewModel()
        {
            _basketItems = new List<BasketItemViewModel>();
        }

        public string UserId { get; set; }
        public string DiscountCode { get; set; }
        //indirim oranı
        public int? DiscountRate { get; set; }
        private List<BasketItemViewModel> _basketItems { get; set; }
        public List<BasketItemViewModel> BasketItems
        {
            get
            {
                if(HasDiscount)
                {
                    _basketItems.ForEach(x =>
                    {
                        var discontPrice = x.Price * ((decimal)DiscountRate.Value / 100);
                        x.AppliedDiscount(Math.Round(x.Price - discontPrice,2));
                    });
                }
                return _basketItems;
                
            }
            set
            {
                _basketItems = value;
            }

        }

        public decimal TotalPrice
        {
            get => _basketItems.Sum(x => x.GetCurrentPrice);
        }
        public bool HasDiscount
        {
            get=> !string.IsNullOrEmpty(DiscountCode);
        }
    }
}
