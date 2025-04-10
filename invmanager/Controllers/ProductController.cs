﻿
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
            products = products.Where(p => p.Quantity < p.Stock);

        // Sort results
        products = sortBy switch
        {
            "Price" => products.OrderBy(p => p.ProductPrice),
            "Quantity" => products.OrderBy(p => p.Quantity),
            "Name" => products.OrderBy(p => p.ProductName),
            _ => products.OrderBy(p => p.ProductName)
        };

        return View("Index", await products.ToListAsync());
    }
     
     
    //create a product  
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
    
    
    //edit existing product 
    [HttpGet]
    public IActionResult Edit(int id)
    {
        //find product by id
        var product = _context.Products.Find(id);
        if (product == null)
        {
            return NotFound();
            
        }
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([Bind("ProductId,ProductName,ProductCategory,ProductPrice,Quantity,Stock")] Product product)
    {
        //ensures validation rules are followed 
        if (!ModelState.IsValid)
        {
            return View(product);
        }

        try
        {
            //update database 
            _context.Products.Update(product);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            //ensures product still exists in database 
            if (!ProductExists(product.ProductId))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    private bool ProductExists(int id)
    {
        return _context.Products.Any(e => e.ProductId == id);
    }

    

    [HttpGet]
    public IActionResult Delete(int id)
    {
        //get product from database using id 
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
            //remove product from database 
            _context.Products.Remove(product);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        return NotFound();
    }
}
