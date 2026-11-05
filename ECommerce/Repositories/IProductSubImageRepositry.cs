namespace ECommerce.Repositories
{
    public interface IProductSubImageRepositry :IRepository<ProductSubImage> 
    {
        void RemoveRange(IEnumerable<ProductSubImage> productSubImages);
    }
}
