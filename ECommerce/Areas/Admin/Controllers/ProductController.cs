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
        //private readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly IRepository<Product> _productRepositry;// = new Repository<Product>();
        private readonly IRepository<Category> _categortRepositry;// = new Repository<Category>();
        private readonly IRepository<Brand> _brandRepositry;// = new Repository<Brand>();
        private readonly IProductSubImageRepositry _productSubImageRepositry;// = new ProductSubImageRepositry();
        private readonly IProductColorRepositry _productColorRepositry;// = new ProductColorRepositry();

        public ProductController(IRepository<Product> productRepositry, IRepository<Category> categortRepositry, IRepository<Brand> brandRepositry, IProductSubImageRepositry productSubImageRepositry, IProductColorRepositry productColorRepositry)
        {
            _productRepositry = productRepositry;
            _categortRepositry = categortRepositry;
            _brandRepositry = brandRepositry;
            _productSubImageRepositry = productSubImageRepositry;
            _productColorRepositry = productColorRepositry;
        }

        public async Task<IActionResult> Index(FilterProductVM filter)
        {
            //var products = _context.Products.AsQueryable();
            var products = await _productRepositry.GetListAsync(includes: [p => p.Category, p => p.Brand]);
            //products = products.Include(p => p.Category).Include(p => p.Brand);

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

            //ViewBag.Categories = _context.Categories;
            ViewBag.Categories = await _categortRepositry.GetListAsync();
            //ViewBag.Brands = _context.Brands;
            ViewBag.Brands = await _brandRepositry.GetListAsync();


            ViewBag.TotalPages = (int)Math.Ceiling(products.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            products = products.Skip((filter.Page - 1) * 8).Take(8);

            return View(products.AsEnumerable());
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            //var categories = _context.Categories.ToList();
            var categories = await _categortRepositry.GetListAsync();
            //var brands = _context.Brands.ToList();
            var brands = await _brandRepositry.GetListAsync();
            return View(new ProductVM
            {
                Categories = categories.ToList(),
                Brands = brands.ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile ImageFile, List<IFormFile> SubImages, List<string> Colors)
        {
            if (ImageFile != null && ImageFile.Length > 0)
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
            //var savedProduct = _context.Products.Add(product);
            var savedProduct = await _productRepositry.AddAsync(product);
            //_context.SaveChanges();
            await _productRepositry.CommitAsync();

            // SubImages
            if (SubImages != null && SubImages.Count > 0)
            {
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

                        //_context.ProductSubImages.Add(new ProductSubImage()
                        //{
                        //    Image = fileName,
                        //    ProductId = savedProduct.Entity.Id
                        //});

                        await _productSubImageRepositry.AddAsync(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = savedProduct.Entity.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _productSubImageRepositry.CommitAsync();
            }

            if (Colors != null && Colors.Count > 0)
            {
                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    //{
                    //    Color = color,
                    //    ProductId = savedProduct.Entity.Id
                    //});

                    await _productColorRepositry.AddAsync(new ProductColor()
                    {
                        Color = color,
                        ProductId = savedProduct.Entity.Id
                    });
                }
                //_context.SaveChanges();
                await _productColorRepositry.CommitAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepositry.GetOneAsync(c => c.Id == id);
            if (product == null)
            {
                return RedirectToAction("NotFoundPage", "Home");
            }

            //var categories = _context.Categories.ToList();
            var categories = await _categortRepositry.GetListAsync();
            //var brands = _context.Brands.ToList();
            var brands = await _brandRepositry.GetListAsync();

            return View(new ProductVM
            {
                Product = product,
                Categories = categories.ToList(),
                Brands = brands.ToList(),
                //ProductSubImages = _context.ProductSubImages.Where(p=>p.ProductId == id).ToList(),
                ProductSubImages = (await _productSubImageRepositry.GetListAsync(p => p.ProductId == id)).ToList(),
                //ProductColors = _context.ProductColors.Where(p=>p.ProductId == id).ToList()
                ProductColors = (await _productColorRepositry.GetListAsync(p => p.ProductId == id)).ToList()
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(Product product, IFormFile ImageFile, List<IFormFile> SubImages, List<string> Colors)
        {
            //var productInDb = _context.Products.AsNoTracking().FirstOrDefault(b => b.Id == product.Id);
            var productInDb = await _productRepositry.GetOneAsync(b => b.Id == product.Id, isTracked: false);
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
            //_context.Products.Update(product);
            _productRepositry.Update(product);
            //_context.SaveChanges();
            await _productRepositry.CommitAsync();

            // SubImages
            if (SubImages != null && SubImages.Count > 0)
            {

                //var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
                var oldSubImages = await _productSubImageRepositry.GetListAsync(p => p.ProductId == product.Id);

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
                // **********
                //_context.ProductSubImages.RemoveRange(oldSubImages);
                _productSubImageRepositry.RemoveRange(oldSubImages);
                //_context.SaveChanges();
                await _productSubImageRepositry.CommitAsync();
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
                        //_context.ProductSubImages.Add(new ProductSubImage()
                        //{
                        //    Image = fileName,
                        //    ProductId = product.Id
                        //});

                        await _productSubImageRepositry.AddAsync(new ProductSubImage()
                        {
                            Image = fileName,
                            ProductId = product.Id
                        });
                    }
                }
                //_context.SaveChanges();
                await _productSubImageRepositry.CommitAsync();
            }



            if (Colors != null && Colors.Count > 0)
            {
                // delete from DB
                //var oldColors = _context.ProductColors.Where(p => p.ProductId == product.Id);
                var oldColors = await _productColorRepositry.GetListAsync(p => p.ProductId == product.Id);
                // **********
                //_context.ProductColors.RemoveRange(oldColors);
                _productColorRepositry.RemoveRange(oldColors);

                foreach (var color in Colors)
                {
                    //_context.ProductColors.Add(new ProductColor()
                    //{
                    //    Color = color,
                    //    ProductId = product.Id
                    //});
                    await _productColorRepositry.AddAsync(new ProductColor()
                    {
                        ProductId = product.Id
                    });
                }
                //_context.SaveChanges();
                await _productColorRepositry.CommitAsync();

            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            //var product = _context.Products.FirstOrDefault(c => c.Id == id);
            var product = await _productRepositry.GetOneAsync(c => c.Id == id);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", product.MainImg);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            //var oldSubImages = _context.ProductSubImages.Where(p => p.ProductId == product.Id);
            var oldSubImages = await _productSubImageRepositry.GetListAsync(p => p.ProductId == product.Id);
            // delete from wwwroot
            foreach (var item in oldSubImages)
            {
                var OldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", item.Image);

                if (System.IO.File.Exists(OldFilePath))
                {
                    System.IO.File.Delete(OldFilePath);
                }
            }

            //_context.Products.Remove(product);
            _productRepositry.Remove(product);
            //_context.SaveChanges();
            await _productRepositry.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
