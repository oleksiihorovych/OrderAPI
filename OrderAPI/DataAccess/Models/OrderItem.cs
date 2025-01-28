using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DataAccess.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; } 

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal UnitPrice { get; set; }
        public int OrderId { get; set; } 
        public Order Order { get; set; }
    }
}