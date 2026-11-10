using System.ComponentModel.DataAnnotations;
using System.Data.Common;

namespace ECommerce.ViewModels
{
    public class RegisterVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }
        [EmailAddress , DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password) , Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
