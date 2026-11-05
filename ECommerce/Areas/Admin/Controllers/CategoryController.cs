using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Category> _categoryRepositry;// = new Repository<Category>();

        public CategoryController(IRepository<Category> categoryRepositry)
        {
            _categoryRepositry = categoryRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //var categories = _context.Categories.AsQueryable();
            var categories = await _categoryRepositry.GetListAsync();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid) // false
            {
                return View(category);
            }
            //_context.Categories.Add(category);
            await _categoryRepositry.AddAsync(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepositry.GetOneAsync(c => c.Id == id);
            if(category == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            //_context.Categories.Update(category);
            _categoryRepositry.Update(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = await _categoryRepositry.GetOneAsync(c => c.Id == id);

            //_context.Categories.Remove(category);
            _categoryRepositry.Remove(category);
            //_context.SaveChanges();
            await _categoryRepositry.CommitAsync();
            
            return RedirectToAction(nameof(Index));
        }



    }
}
