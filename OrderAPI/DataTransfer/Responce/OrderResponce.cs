using OrderAPI.DataAccess.Models;

namespace OrderAPI.DataTransfer
{
    public class OrderResponse
    {
        public int OrderNumber { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; }

        public List<OrderItemResponse> OrderItems { get; set; }
    }

    public class OrderItemResponse
    {
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
