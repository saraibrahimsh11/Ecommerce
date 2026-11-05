using ECommerce.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class ProductColorRepositry : Repository<ProductColor> , IProductColorRepositry
    {
        public ProductColorRepositry(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.RemoveRange(productColors);
        }
    }
}
