using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var categories = _context.Categories.AsQueryable();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            
            _context.Categories.Remove(category);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }



    }
}
