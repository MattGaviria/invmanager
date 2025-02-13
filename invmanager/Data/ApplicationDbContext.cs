using invmanager.Models;
using Microsoft.EntityFrameworkCore;

namespace invmanager.Data;

public class ApplicationDbContext : DbContext
{
   public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } 

   public DbSet<Category> Categories { get; set; }
   public DbSet<Product> Products { get; set; }
   public DbSet<Order> Orders { get; set; }
   public DbSet<OrderProduct> OrderProducts { get; set; } 

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
      
      // seeding the category table
      modelBuilder.Entity<Category>().HasData(
         new Category {CategoryId = 1, CategoryName = "Electronics", CategoryDescription = "Devices and gadgets for daily use, including phones, computers, and accessories."},
         new Category {CategoryId = 2, CategoryName = "Clothing", CategoryDescription = "Apparel and fashion items for men, women, and children."},
         new Category {CategoryId = 3, CategoryName = "Books", CategoryDescription = "Printed or digital reading materials across various genres and topics."},
         new Category {CategoryId = 4, CategoryName = "Toys", CategoryDescription = "Items for children, including games, dolls, and educational toys."},
         new Category {CategoryId = 5, CategoryName = "Furniture", CategoryDescription = "Home and office furnishings, such as chairs, tables, and storage units."},
         new Category {CategoryId = 6, CategoryName = "Other", CategoryDescription = "Miscellaneous items that don't fit into the above categories."}
      );
      
      // seeding the product table
      modelBuilder.Entity<Product>().HasData(
         new Product {ProductId = 1, ProductName = "HP Laptop", ProductCategory = "Electronics", CategoryId = 1, ProductPrice = 799.99, Quantity = 15}, 
         new Product {ProductId = 2, ProductName = "Wooden Shelf", ProductCategory = "Furniture", CategoryId = 5, ProductPrice = 1199.99, Quantity = 5},
         new Product {ProductId = 3, ProductName = "Remote Control Race Car", ProductCategory = "Toys", CategoryId = 4, ProductPrice = 49.99, Quantity = 20}
      );
      
      // creating the one-to-many relationship between Category and Product
      modelBuilder.Entity<Product>()
         .HasOne(p => p.Category)
         .WithMany(c => c.Products)
         .HasForeignKey(p => p.CategoryId);
      
      // creating a one-to-many relationship between Order and OrderProduct
      modelBuilder.Entity<OrderProduct>()
         .HasKey(op => new { op.OrderId, op.ProductId });
      modelBuilder.Entity<OrderProduct>()
         .HasOne(op => op.Order)
         .WithMany(o => o.OrderProducts)
         .HasForeignKey(op => op.OrderId);
      
      // creating a one-to-many relationship between Product and OrderProduct
      modelBuilder.Entity<OrderProduct>()
         .HasOne(op => op.Product)
         .WithMany(p => p.OrderProducts)
         .HasForeignKey(op => op.ProductId);
      
   }
   
}