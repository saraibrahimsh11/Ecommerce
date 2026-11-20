using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;// = new ApplicationDbContext();
        private readonly IRepository<Product> _productRepository;

        public HomeController(ApplicationDbContext context, IRepository<Product> productRepository)
        {
            _context = context;
            _productRepository = productRepository;
        }

        public async Task<IActionResult> Index(FilterProductVM filter)
        {
            //var products = _context.Products.AsQueryable();
            var products = await _productRepository.GetListAsync(includes: [p => p.Category]);

            if(filter.ProductName != null)
            {
                products = products.Where(p => p.Name.Contains(filter.ProductName));
            }

            if (filter.MinPrice > 0)
            {
                products = products.Where(p => p.Price >= filter.MinPrice);
            }

            if (filter.MaxPrice > 0)
            {
                products = products.Where(p => p.Price <= filter.MaxPrice);
            }

            if(filter.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
            }

            if(filter.IsHot)
            {
                products = products.Where(p => p.Discount > 40);
            }

            ViewBag.Categories = _context.Categories;
            ViewBag.Brands = _context.Brands; 
            
            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1) * 8).Take(8);
            return View(products.ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ProductDedails(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id , includes: [p=>p.Category]);
            if(product == null)
            {
                return NotFound();
            }

            var relatedProducts = await _productRepository.GetListAsync(
                p => p.CategoryId == product.CategoryId &&
                p.Id != product.Id
                );
            relatedProducts = relatedProducts.Skip(0).Take(4);
            return View(new ProductWithRelatedVM()
            {
                Product = product,
                RelatedProducts = relatedProducts
            });
        }

        public IActionResult PersonalInfo(decimal amount , string name)
        {
            var personsInDb = new List<Person>()
            {
                new Person (){ Id = 1 ,  Name = "Ahmed" , Address = "Cairo" , Salary = 10000},
                new Person (){ Id = 2 ,  Name = "Omer" , Address = "Alex" , Salary = 20000},
                new Person (){ Id = 3 ,  Name = "Mona" , Address = "Giza" , Salary = 30000}
            };

            var persons = personsInDb.Where(p => p.Salary > amount);
            if(name != null)
                persons = persons.Where(p => p.Name.Contains(name));

            int count = persons.Count();

            return View(new PersonVM
            {
                Persons = persons , 
                Count = count
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
