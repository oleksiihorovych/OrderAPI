// using OrderAPI.Services.Abstractions;
// using RabbitMQ.Client;
// using System.Text;
// using System.Text.Json;

// namespace OrderAPI.Services
// {
//     public class PaymentPublisher : IPaymentPublisher
//     {
//         private readonly string _hostname = "localhost";
//         private readonly string _queueName = "payment_updates";
//         private readonly ConnectionFactory _connectionFactory;

//         public PaymentPublisher()
//         {
//             _connectionFactory = new ConnectionFactory()
//             {
//                 HostName = _hostname,
//                 UserName = "guest", 
//                 Password = "guest"
//             };
//         }

//         async public void PublishPaymentStatus(int orderNumber, bool isPaid)
//         {
//             using var connection = await _connectionFactory.CreateConnectionAsync();
//             using var channel = await connection.CreateChannelAsync();

//             await channel.QueueDeclareAsync(
//                 queue: _queueName,
//                 durable: true,
//                 exclusive: false,
//                 autoDelete: false,
//                 arguments: null);

//             var paymentStatus = new { OrderNumber = orderNumber, IsPaid = isPaid };
//             var message = JsonSerializer.Serialize(paymentStatus);
//             var body = Encoding.UTF8.GetBytes(message);
            
//             await channel.BasicPublishAsync(
//                 exchange: "",
//                 routingKey: _queueName,
//                 body: body);

//             Console.WriteLine($" [x] Sent '{message}'");
//         }
//     }
// }