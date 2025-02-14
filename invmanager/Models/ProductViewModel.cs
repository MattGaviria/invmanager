using System.Collections.Generic;

namespace invmanager.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }
        public List<Category> Categories { get; set; }
    }

}