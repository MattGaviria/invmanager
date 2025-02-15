using invmanager.Models;
using Microsoft.EntityFrameworkCore;

namespace invmanager.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { } 

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderProduct> OrderProducts { get; set; } // Added DbSet for OrderProduct

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define many-to-many relationship between Order and Product using OrderProduct
        modelBuilder.Entity<OrderProduct>()
            .HasKey(op => op.OrderProductId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Order)
            .WithMany(o => o.OrderProducts)
            .HasForeignKey(op => op.OrderId);

        modelBuilder.Entity<OrderProduct>()
            .HasOne(op => op.Product)
            .WithMany(p => p.OrderProducts)
            .HasForeignKey(op => op.ProductId);

        modelBuilder.Entity<Product>().HasData(
            new Product { ProductId = 1, ProductName =  "Laptop", ProductCategory = "Electronics", ProductPrice = 999.99, Quantity = 5, Stock = 5 },
            new Product { ProductId = 2, ProductName =  "Smartphone", ProductCategory =  "Electronics", ProductPrice = 699.99, Quantity = 15, Stock = 15 },
            new Product { ProductId = 3, ProductName =  "T-Shirt", ProductCategory =  "Clothing", ProductPrice = 19.99, Quantity = 50, Stock = 50 }
        );
    }
}