using System.ComponentModel.DataAnnotations;

namespace OrderAPI.DataTransfer
{
    public class OrderRequest
    {
        [Required(ErrorMessage = "Order number is required.")]
        public int OrderNumber { get; set; }

        [Required(ErrorMessage = "Customer name is required.")]
        public string CustomerName { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [MinLength(1, ErrorMessage = "An order must contain at least one item.")]
        public List<OrderItemRequest> OrderItems { get; set; }

    }

    public class OrderItemRequest
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal UnitPrice { get; set; }
    }
}
