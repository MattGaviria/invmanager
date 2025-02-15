using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using invmanager.Models;
using invmanager.Data;


namespace invmanager.Controllers;

public class ProductController : Controller
{ 
    private readonly ApplicationDbContext _context;

    public ProductController(ApplicationDbContext context)
    {
        _context = context;
    }
  

    [HttpGet] //Display products
    public async Task<IActionResult> Index()
    {
        return View(await _context.Products.ToListAsync());
    }
    
    [HttpGet]
     // search method
    public async Task<IActionResult> SearchAndFilter(string query, string category, double? minPrice, double? maxPrice, bool lowStock, string sortBy)
    {
        var products = _context.Products.AsQueryable();

        // Search by name or category
        if (!string.IsNullOrEmpty(query))
            products = products.Where(p => p.ProductName.Contains(query) || p.ProductCategory.Contains(query));

        // Filter by category
        if (!string.IsNullOrEmpty(category) && category != "All Categories")
            products = products.Where(p => p.ProductCategory == category);

        // Filter by price range
        if (minPrice.HasValue)
            products = products.Where(p => p.ProductPrice >= minPrice);

        if (maxPrice.HasValue)
            products = products.Where(p => p.ProductPrice <= maxPrice);

        // Show only low-stock products
        if (lowStock)
            products = products.Where(p => p.Quantity < 10);

        // Sort results
        products = sortBy switch
        {
            "Price" => products.OrderBy(p => p.ProductPrice),
            "Quantity" => products.OrderBy(p => p.Quantity),
            "Name" => products.OrderBy(p => p.ProductName),
            _ => products.OrderBy(p => p.ProductName)
        };

        return View("SearchAndFilter", await products.ToListAsync());
    }
     
     
     
    [HttpGet]
    public IActionResult Create()
    {
        //list of categories for products 
        return View();
 
    }

    [HttpPost]
    [ValidateAntiForgeryToken]

    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            //add new product 
            _context.Products.Add(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
            
        } 
        return View(product);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
            
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, [Bind("ProductId, ProductName, ProductCategory, ProductPrice, Quantity, Stock")] Product product)
    {
        
        {
            // Print to console for debugging purposes
            Console.WriteLine("Edit method called with id: " + id);

            if (id != product.ProductId)
            {
                Console.WriteLine("ProductId mismatch. id: " + id + ", Product.ProductId: " + product.ProductId);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Console.WriteLine("Updating product: " + product);
                    _context.Update(product);
                    _context.SaveChanges();
                    Console.WriteLine("Product updated successfully.");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("Concurrency exception occurred: " + ex.Message);
                    if (!ProductExists(product.ProductId))
                    {
                        Console.WriteLine("Product not found. ProductId: " + product.ProductId);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            Console.WriteLine("ModelState is not valid. Errors: " + string.Join(", ", ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
            ViewBag.Categories = _context.Categories.ToList(); // Repopulate categories in case of validation failure
            return View(product);
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }


    [HttpGet]
    public IActionResult Delete(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]

    public IActionResult DeleteConfirmed(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return NotFound();
    }
}
