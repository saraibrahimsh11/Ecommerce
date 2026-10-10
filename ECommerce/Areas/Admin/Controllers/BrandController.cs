using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index()
        {
            var categories = _context.Brands.AsQueryable();
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Brand brand , IFormFile ImageFile)
        {
            if(ImageFile != null && ImageFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                brand.Logo = fileName;
            }
            _context.Brands.Add(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            if(brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            return View(brand);
        }

        [HttpPost]
        public IActionResult Update(Brand brand , IFormFile ImageFile)
        {
            var brandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brandInDb.Logo);

                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                }

                brand.Logo = fileName;
            }
            else
            {
                brand.Logo = brandInDb.Logo;
            }
            _context.Brands.Update(brand);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var brand = _context.Brands.FirstOrDefault(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Logo);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Brands.Remove(brand);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }



    }
}
