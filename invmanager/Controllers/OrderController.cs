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
            if (products == null || !products.Any())
            {
                return NotFound("No products available.");
            }
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToOrderConfirmed(string CustomerName, string CustomerEmail, int ProductId, int Quantity)
        {
            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(CustomerEmail))
            {
                return BadRequest("Customer name and email are required.");
            }

            TempData["CustomerName"] = CustomerName;
            TempData["CustomerEmail"] = CustomerEmail;

            var product = _context.Products.Find(ProductId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            var order = _context.Orders.Include(o => o.OrderProducts)
                .FirstOrDefault(o => o.Status == "InProgress" && o.CustomerName == CustomerName && o.CustomerEmail == CustomerEmail);

            if (order == null)
            {
                order = new Order
                {
                    Status = "InProgress",
                    CustomerName = CustomerName,
                    CustomerEmail = CustomerEmail,
                    OrderProducts = new List<OrderProduct>()
                };
                _context.Orders.Add(order);
                _context.SaveChanges();
            }

            if (order.OrderProducts == null)
            {
                order.OrderProducts = new List<OrderProduct>();
            }

            var existingOrderProduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == ProductId);
            if (existingOrderProduct != null)
            {
                existingOrderProduct.Quantity += Quantity;
            }
            else
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.OrderId,
                    ProductId = product.ProductId,
                    Quantity = Quantity
                };
                _context.OrderProducts.Add(orderProduct);
                order.OrderProducts.Add(orderProduct);
            }

            _context.SaveChanges();

            TempData["SuccessMessage"] = "Product added to order successfully!";
            return RedirectToAction("Index", new { CustomerName, CustomerEmail });
        }

        [HttpGet]
        public IActionResult SummaryOfOrder(string CustomerName, string CustomerEmail)
        {
            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(CustomerEmail))
            {
                return BadRequest("Customer name and email are required.");
            }

            var orders = _context.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product)
                .Where(o => o.CustomerName == CustomerName && o.CustomerEmail == CustomerEmail)
                .ToList();

            if (orders == null || !orders.Any())
            {
                return NotFound($"No orders found for customer {CustomerName}.");
            }

            // Check if there are any unconfirmed orders
            ViewBag.HasUnconfirmedOrders = orders.Any(o => o.Status == "InProgress");
            ViewBag.CustomerName = CustomerName;
            return View(orders);
        }

        [HttpGet]
        public IActionResult ConfirmOrder(int OrderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .FirstOrDefault(o => o.OrderId == OrderId);

            if (order == null)
            {
                return NotFound("Order not found.");
            }

            order.Status = "Confirmed";
            _context.SaveChanges();

            return RedirectToAction("SummaryOfOrder", new { CustomerName = order.CustomerName, CustomerEmail = order.CustomerEmail });
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromOrder(int OrderId, int ProductId)
        {
            var order = _context.Orders.Include(o => o.OrderProducts).FirstOrDefault(o => o.OrderId == OrderId);
            if (order == null)
            {
                return NotFound("Order not found.");
            }

            var orderProduct = order.OrderProducts.FirstOrDefault(op => op.ProductId == ProductId);
            if (orderProduct == null)
            {
                return NotFound("Product not found in order.");
            }

            _context.OrderProducts.Remove(orderProduct);

            if (!order.OrderProducts.Any())
            {
                _context.Orders.Remove(order);
            }

            _context.SaveChanges();

            return RedirectToAction("SummaryOfOrder", new { CustomerName = order.CustomerName, CustomerEmail = order.CustomerEmail });
        }
    }
}

    
