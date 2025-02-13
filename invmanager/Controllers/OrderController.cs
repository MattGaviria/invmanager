using Microsoft.AspNetCore.Mvc;
using invmanager.Models;
using invmanager.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace invmanager.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }
        




 
        
        



    }

}
