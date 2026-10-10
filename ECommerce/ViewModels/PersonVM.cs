using ECommerce.Models;

namespace ECommerce.ViewModels
{
    public class PersonVM
    {
        public IEnumerable<Person> Persons { get; set; }
        public int Count { get; set; }
    }
}
