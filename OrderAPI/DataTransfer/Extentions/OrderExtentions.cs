using OrderAPI.Constants;
using OrderAPI.DataAccess.Models;

namespace OrderAPI.DataTransfer.Extentions
{
    public static class OrderExtensions
    {
        public static Order ToModel(this OrderRequest orderRequest)
        {
            return new Order
            {
                OrderNumber = orderRequest.OrderNumber,
                CustomerName = orderRequest.CustomerName,
                OrderDate = orderRequest.OrderDate,
                // Status = OrderStatusDictionary.New,
                Status = "New",
                OrderItems = orderRequest.OrderItems.Select(item => new OrderItem
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }

        public static OrderResponse ToResponse(this Order order)
        {
            return new OrderResponse
            {
                OrderNumber = order.OrderNumber,
                CustomerName = order.CustomerName,
                OrderDate = order.OrderDate,
                Status = order.Status,
                OrderItems = order.OrderItems.Select(item => new OrderItemResponse
                {
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                }).ToList()
            };
        }
    }
}
