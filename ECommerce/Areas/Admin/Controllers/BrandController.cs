using ECommerce.Data;
using ECommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Brand> _brandRepositry;// = new Repository<Brand>();

        public BrandController(IRepository<Brand> brandRepositry)
        {
            _brandRepositry = brandRepositry;
        }

        public async Task<IActionResult> Index()
        {
            //var brands = _context.Brands.AsQueryable();
            var brands = await _brandRepositry.GetListAsync();
            return View(brands.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand , IFormFile ImageFile)
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
            //_context.Brands.Add(brand);
            await _brandRepositry.AddAsync(brand);
            //_context.SaveChanges();
            await _brandRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand =  await _brandRepositry.GetOneAsync(c => c.Id == id);
            if(brand == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }
            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Brand brand , IFormFile ImageFile)
        {
            //var brandInDb = _context.Brands.AsNoTracking().FirstOrDefault(b => b.Id == brand.Id);
            var brandInDb = await _brandRepositry.GetOneAsync(b => b.Id == brand.Id, isTracked: false);
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
            //_context.Brands.Update(brand);
            _brandRepositry.Update(brand);
            //_context.SaveChanges();
            await _brandRepositry.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var brand = _context.Brands.FirstOrDefault(c => c.Id == id);
            var brand = await _brandRepositry.GetOneAsync(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", brand.Logo);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //_context.Brands.Remove(brand);
            _brandRepositry.Remove(brand);
            //_context.SaveChanges();
            await _brandRepositry.CommitAsync();
            
            return RedirectToAction(nameof(Index));
        }

    }
}
