using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invmanager.Models;

public class Order
{
    public int OrderId { get; set; }
    
    public string CustomerName { get; set;}

    public string CustomerEmail { get; set;}
    
    [DataType(DataType.Date)]
    public DateTime OrderDate { get; set; }
    
    public double OrderTotal { get; set; }

    public string OrderStatus { get; set; }
    
    public ICollection<OrderProduct> OrderProducts { get; set; } // navigation property
    
}