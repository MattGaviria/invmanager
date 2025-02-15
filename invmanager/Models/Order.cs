using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace invmanager.Models
{
    public class Order 
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public  int Quantity { get; set; }

        public double OrderTotal { get; set; } 

        [Required]
        public string Status { get; set; } = "Pending"; 

        [Required]
        
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        
        public ICollection<OrderProduct>? OrderProducts { get; set; }
    }
}