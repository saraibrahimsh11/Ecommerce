using ECommerce.Data;
using ECommerce.Models;
using ECommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Net.WebRequestMethods;

namespace ECommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context = new ApplicationDbContext();
        public IActionResult Index(FilterProductVM filter)
        {
            var products = _context.Products.AsQueryable();
            products = products.Include(p => p.Category).Include(p => p.Brand);

            if (filter.ProductName != null)
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

            if (filter.CategoryId > 0)
            {
                products = products.Where(p => p.CategoryId == filter.CategoryId);
            }

            if (filter.BrandId > 0)
            {
                products = products.Where(p => p.BrandId == filter.BrandId);
            }

            if (filter.IsLowQuantity)
            {
                products = products.OrderBy(p => p.Quantity);
                ViewBag.IsLowQuantity = filter.IsLowQuantity;
            }

            ViewBag.Categories = _context.Categories;
            ViewBag.Brands = _context.Brands;

            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1) * 8).Take(8);

            return View(products.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();
            return View(new ProductVM
            {
                Categories = categories,
                Brands = brands
            });
        }

        [HttpPost]
        public IActionResult Create(Product product , IFormFile ImageFile , List<IFormFile> SubImages , List<string> Colors)
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

                product.MainImg = fileName;
            }
            var savedProduct = _context.Products.Add(product);
            _context.SaveChanges();

            // SubImages
            if(SubImages != null && SubImages.Count > 0)
            {
                foreach(var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in Database
                        _context.ProductSubImages.Add(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = savedProduct.Entity.Id
                        });
                    }
                }
                _context.SaveChanges();
            }

            if(Colors != null && Colors.Count > 0)
            {
                foreach(var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        Color = color,
                        ProductId = savedProduct.Entity.Id
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            var categories = _context.Categories.ToList();
            var brands = _context.Brands.ToList();

            return View(new ProductVM
            {
                Product = product,
                Categories = categories,
                Brands = brands,
                ProductSubImages = _context.ProductSubImages.Where(p=>p.ProductId == id).ToList(),
                ProductColors = _context.ProductColors.Where(p=>p.ProductId == id).ToList()
            });
        }

        [HttpPost]
        public IActionResult Update(Product product , IFormFile ImageFile, List<IFormFile> SubImages, List<string> Colors)
        {
            var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            if (ImageFile != null && ImageFile.Length > 0)
            {
                //var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var fileName = Guid.NewGuid().ToString() + "-" + ImageFile.FileName;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    ImageFile.CopyTo(stream);
                }

                var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", productInDb.MainImg);

                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                }

                product.MainImg = fileName;
            }
            else
            {
                product.MainImg = productInDb.MainImg;
            }
            _context.Products.Update(product);
            _context.SaveChanges();

            // SubImages
            if (SubImages != null && SubImages.Count > 0)
            {
                
                var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);

                // delete from wwwroot
                foreach (var item in oldSubImages)
                {
                    var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.Image);

                    if (System.IO.File.Exists(OldFilePath))
                    {
                        System.IO.File.Delete(OldFilePath);
                    }
                }

                // delete from DB
                _context.ProductSubImages.RemoveRange(oldSubImages);
                _context.SaveChanges();
                foreach (var image in SubImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        // insert in wwwroot
                        var fileName = Guid.NewGuid().ToString() + "-" + image.FileName;

                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            image.CopyTo(stream);
                        }
                        // insert in Database
                        _context.ProductSubImages.Add(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                _context.SaveChanges();
            }

            

            if (Colors != null && Colors.Count > 0)
            {
                // delete from DB
                var oldColors = _context.ProductColors.Where(p => p.ProductId == product.Id);
                _context.ProductColors.RemoveRange(oldColors);

                foreach (var color in Colors)
                {
                    _context.ProductColors.Add(new ProductColor()
                    {
                        Color = color,
                        ProductId = product.Id
                    });
                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var product = _context.Products.FirstOrDefault(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);

            // delete from wwwroot
            foreach (var item in oldSubImages)
            {
                var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.Image);

                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                }
            }

            _context.Products.Remove(product);
            _context.SaveChanges();
            
            return RedirectToAction(nameof(Index));
        }



    }
}
