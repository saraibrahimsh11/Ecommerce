using ECommerce.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Category
    {
        public int Id { get; set; }
        //[MinLength(2)]
        //[MaxLength(30)]
        [MinMaxLengthCustom(2,30)]
        public string Name { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
