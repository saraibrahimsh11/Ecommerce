namespace ECommerce.ViewModels
{
    public class ProductWithRelatedVM
    {
        public Product Product { get; set; }
        public IEnumerable<Product> RelatedProducts { get; set; }
    }
}
