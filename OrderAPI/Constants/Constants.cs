namespace OrderAPI.Constants
{
    public class Constants
    {
        public enum OrderStatus
        {
            New,
            Paid,
            Canceled
        }

        public static readonly string NewOrder = "New Order";
        public static readonly string PaidOrder = "Paid Order";
        public static readonly string CanceledOrder = "Canceled Order";

        public static readonly Dictionary<OrderStatus, string> StatusDictionary = new Dictionary<OrderStatus, string>
        {
            { OrderStatus.New, NewOrder },
            { OrderStatus.Paid, PaidOrder },
            { OrderStatus.Canceled, CanceledOrder }
        };
    }
}
