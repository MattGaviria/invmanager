using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invmanager.Models;

public class Product
{
    public int ProductId { get; set; }
    
    [Required]

    public string ProductName { get; set; }

    [Required]
    
    public string ProductCategory { get; set; }
    
    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    
    public double ProductPrice { get; set; }
    
    public int Quantity { get; set; }

    public int? Stock { get; set; }

    public Category Category { get; set; } // navigation property
    
    public ICollection<OrderProduct> OrderProducts { get; set; } // navigation property
}