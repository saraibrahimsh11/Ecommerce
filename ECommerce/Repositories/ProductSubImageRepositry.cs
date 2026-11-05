using ECommerce.Data;
using System.Collections;

namespace ECommerce.Repositories
{
    public class ProductSubImageRepositry : Repository<ProductSubImage> , IProductSubImageRepositry
    {
        public ProductSubImageRepositry(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange (IEnumerable<ProductSubImage> productSubImages)
        {
            _context.RemoveRange(productSubImages);
        }
    }
}
