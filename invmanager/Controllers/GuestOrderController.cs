using invmanager.Models;
using Microsoft.AspNetCore.Mvc;
using invmanager.Data;
using Microsoft.EntityFrameworkCore;

namespace invmanager.Controllers
{
    public class GuestOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        //  Display all products for selection
        [HttpGet]
        public IActionResult Index()
        {
            var products = _context.Products.ToList(); // Fetch all products
            return View(products);
        }

        //  Create or retrieve an existing "Pending" order
        private Order GetOrCreateOrder()
        {
            // Check if there is an ongoing order in the database.
            var order = _context.Orders
                .Where(o => o.OrderStatus == "Pending") // Filter for "Pending" orders
                .Include(o => o.OrderProducts)  // Include the related OrderProducts
                .ThenInclude(op => op.Product)  // Also include the related Product data for each OrderProduct
                .FirstOrDefault();

            if (order == null)
            {
                order = new Order { OrderStatus = "Pending", OrderProducts = new List<OrderProduct>() };
                _context.Orders.Add(order);
                _context.SaveChanges(); // Save the newly created order
            }

            return order;
        }


        // Add selected products to the guest order
        [HttpPost]
        public IActionResult CreateOrder(int[] productIds, int[] quantities)
        {
            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                return BadRequest("Invalid product selection.");
            }

            var order = GetOrCreateOrder(); 

            for (int i = 0; i < productIds.Length; i++)
            {
                int productId = productIds[i];
                int quantity = quantities[i];

                var product = _context.Products.Find(productId);
                if (product == null) continue;

                var existingItem = order.OrderProducts.FirstOrDefault(op => op.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.Price = product.ProductPrice * existingItem.Quantity;
                }
                else
                {
                    order.OrderProducts.Add(new OrderProduct
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.ProductPrice * quantity
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Summary");
        }

        // Confirm the order with optional guest name & email
        [HttpPost]
        public IActionResult ConfirmOrder(string? name, string? email)
        {
            var order = GetOrCreateOrder();

            order.CustomerName = name ?? "Guest";
            order.CustomerEmail = email;
            order.OrderDate = DateTime.UtcNow;
            order.OrderStatus = "Placed";

            // Loop through each OrderProduct in the order
            foreach (var orderProduct in order.OrderProducts)
            {
                var product = _context.Products.Find(orderProduct.ProductId);
                if (product != null)
                {
                    // Decrease the stock by the quantity in the order
                    product.Quantity -= orderProduct.Quantity;

                    // Make sure the product quantity doesn't go below zero
                    if (product.Quantity < 0)
                    {
                        // Handle the case where stock goes negative (e.g., return error or set to zero)
                        product.Quantity = 0;
                    }
                }
            }

            // Save changes to the product stock
            _context.SaveChanges(); 

         

            return RedirectToAction("Summary");
        }

        //  Display the summary of the guest order
        [HttpGet]
        public IActionResult Summary()
        {
            var order = GetOrCreateOrder();
            return View(order);
        }
        
        [HttpPost]
        public IActionResult AddItem(int productId, int quantity)
        {
            var order = GetOrCreateOrder();

            var product = _context.Products.Find(productId);
            if (product == null) return BadRequest("Product not found");

            // Ensure there's enough stock available
            if (product.Quantity < quantity)
            {
                return BadRequest("Not enough stock available.");
            }

            var existingItem = order.OrderProducts.FirstOrDefault(op => op.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.Price = product.ProductPrice * existingItem.Quantity;
            }
            else
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    Price = product.ProductPrice * quantity
                });
            }

            // Update the product's available quantity
            product.Quantity -= quantity;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        
        public IActionResult OrderHistory()
        {
            // Retrieve all orders, including related OrderProducts and Products
            var orders = _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product) // Include related products for each order
                .ToList();

            return View(orders); // Pass the orders list to the view
        }


    }
}
