using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Models
{
    [PrimaryKey(nameof(Image), nameof(ProductId))]

    public class ProductSubImage
    {
        public string Image { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
    }
}
