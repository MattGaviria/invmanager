using invmanager.Models;
using Microsoft.EntityFrameworkCore;

namespace invmanager.Data;

public class ApplicationDbContext : DbContext
{
   public object Projects;
   public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } 
   
   public DbSet<Product> Products { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);
      modelBuilder.Entity<Product>().HasData(
         new Product { ProductId = 1, ProductName =  "Laptop", ProductCategory = "Electronics", ProductPrice = 999.99, Quantity = 5 },
         new Product { ProductId = 2, ProductName =  "Smartphone", ProductCategory =  "Electronics", ProductPrice = 699.99, Quantity = 15 },
         new Product { ProductId = 3, ProductName =  "T-Shirt", ProductCategory =  "Clothing", ProductPrice = 19.99, Quantity = 50 }
      );
   }
   
   
    
}