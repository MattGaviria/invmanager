using System.ComponentModel.DataAnnotations.Schema;

namespace invmanager.Models;
using System.ComponentModel.DataAnnotations;

public class Category
{
    
    public int CategoryId { get; set; }
    
    [Required]
    public required string CategoryName { get; set; }
    
    public string? CategoryDescription { get; set; }
    
    public List<Product>? Products { get; set; } // navigation property
}