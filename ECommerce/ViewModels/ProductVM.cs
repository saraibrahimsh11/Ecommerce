using ECommerce.Models;

namespace ECommerce.ViewModels
{
    public class ProductVM
    {
        public Product? Product { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Brand>? Brands { get; set; }
        public List<ProductSubImage>? ProductSubImages { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
    }
}
