// using RabbitMQ.Client;
// using RabbitMQ.Client.Events;
// using System.Text;
// using System.Text.Json;
// using OrderAPI.DataAccess;
// using Microsoft.EntityFrameworkCore;
// using OrderAPI.Constants;
// using System.Linq;

// namespace OrderAPI.Services
// {
//     public class PaymentConsumer : BackgroundService
//     {
//         private readonly string _hostname = "localhost";
//         private readonly string _queueName = "payment_updates";
//         private readonly ConnectionFactory _connectionFactory;
//         private readonly IServiceProvider _serviceProvider;
//         private readonly ILogger<PaymentConsumer> _logger;

//         public PaymentConsumer(IServiceProvider serviceProvider, ILogger<PaymentConsumer> logger)
//         {
//             _connectionFactory = new ConnectionFactory()
//             {
//                 HostName = _hostname,
//                 UserName = "guest", 
//                 Password = "guest"
//             };
//             _serviceProvider = serviceProvider;
//             _logger = logger;
//         }

//         async protected override Task ExecuteAsync(CancellationToken stoppingToken)
//         {
//             using var connection = await _connectionFactory.CreateConnectionAsync();
//             using var channel = await connection.CreateChannelAsync();

//             await channel.QueueDeclareAsync(
//                     queue: _queueName,
//                     durable: true,
//                     exclusive: false,
//                     autoDelete: false,
//                     arguments: null);

//             var consumer = new AsyncEventingBasicConsumer(channel);
//             consumer.ReceivedAsync += async (model, ea) =>
//             {
//                 try
//                 {
//                     var body = ea.Body.ToArray();
//                     var message = Encoding.UTF8.GetString(body);
//                     var paymentStatus = JsonSerializer.Deserialize<PaymentStatus>(message);

//                     using (var scope = _serviceProvider.CreateScope())
//                     {
//                         var dbContext = scope.ServiceProvider.GetRequiredService<DbAccess>();
//                         var order = await dbContext.Orders.FirstOrDefaultAsync(o => o.OrderNumber == paymentStatus.OrderNumber);

//                         if (order != null)
//                         {
//                             order.Status = paymentStatus.IsPaid ? OrderStatus.PaidOrder : OrderStatus.CancelledOrder;
//                             dbContext.Orders.Update(order);
//                             await dbContext.SaveChangesAsync();
//                         }
//                     }
//                 }
//                 catch (Exception ex)
//                 {
//                     _logger.LogInformation($"Error during message proccess: {ex.Message}");
//                 }
//             };

//             await channel.BasicConsumeAsync(queue: "paymentQueue", autoAck: true, consumer: consumer);
//         }
//         private class PaymentStatus
//         {
//             public int OrderNumber { get; set; }
//             public bool IsPaid { get; set; }
//         }
//     }
// }