using System.ComponentModel.DataAnnotations.Schema;

namespace invmanager.Models;

public class OrderProduct
{
    /// <summary>
    /// this is the join table to represent the many-to-many relationship between Product and Order
    /// because one order can have many products and one product can appear in many orders 
    /// </summary>
    [ForeignKey("Order")]
    public int OrderId { get; set;}
    
    [ForeignKey("Product")]
    public int ProductId { get; set;}
    
    public int Quantity { get; set;}
    
    public Order Order { get; set;} // navigation property
    
    public Product Product { get; set;} // navigation property
    
}