// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using System.Text;

// namespace OrderAPI
// {
//     public class PaymentProcessingService : IPaymentProcessingService
//     {
//         private readonly string _queueName = "paymentQueue";
//         private readonly IConnection _connection;
//         private readonly IModel _channel;

//         public PaymentProcessingService(string hostName)
//         {
//             var factory = new ConnectionFactory() { HostName = hostName };
//             _connection = factory.CreateConnection();
//             _channel = _connection.CreateModel();

//             _channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
//         }

//         public void ListenForPaymentUpdates()
//         {
//             var consumer = new EventingBasicConsumer(_channel);
//             consumer.Received += async (model, ea) =>
//             {
//                 var body = ea.Body.ToArray();
//                 var message = Encoding.UTF8.GetString(body);
//                 int orderId = int.Parse(message);  // Get order ID from the message
                
//                 await ProcessPayment(orderId);
//             };

//             _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
//         }

//         public void PublishPaymentUpdate(int orderId)
//         {
//             var body = Encoding.UTF8.GetBytes(orderId.ToString());

//             _channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);
//         }

//         private async Task ProcessPayment(int orderId)
//         {
//             // Implement logic for payment processing
//             // For example, check payment status from an external service
//             // and update the order accordingly

//             Console.WriteLine($"Processing payment for order {orderId}");
//             await Task.Delay(1000); // Simulating delay

//             // Once processed, update order status
//             var order = await _dbContext.Orders.FindAsync(orderId);
//             if (order != null)
//             {
//                 order.Status = OrderStatus.Paid; // For example
//                 _dbContext.Orders.Update(order);
//                 await _dbContext.SaveChangesAsync();
//                 Console.WriteLine($"Order {orderId} payment processed successfully.");
//             }
//         }
//     }
// }
