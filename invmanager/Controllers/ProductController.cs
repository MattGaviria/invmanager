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
    [HttpGet]
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
}
