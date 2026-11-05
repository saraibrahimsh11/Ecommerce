namespace ECommerce.Repositories
{
    public interface IProductColorRepositry : IRepository<ProductColor>
    {
        void RemoveRange(IEnumerable<ProductColor> productColors);
    }
}
