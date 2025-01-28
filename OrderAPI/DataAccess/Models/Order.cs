using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DataAccess.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        
        [Required]
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; }
        
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; } 

        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
        public List<OrderItem> OrderItems { get; set; }
    }
}