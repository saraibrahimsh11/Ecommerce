namespace ECommerce.ViewModels
{
    public class FilterProductVM
    {
        public string ProductName { get; set; }
        public int MinPrice { get; set; }
        public int MaxPrice { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public bool IsHot { get; set; }
        public bool IsLowQuantity { get; set; }
        public int Page { get; set; } = 1;
    }
}
