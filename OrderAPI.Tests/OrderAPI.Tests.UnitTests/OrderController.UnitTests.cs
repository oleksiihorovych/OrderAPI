using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using OrderAPI.Controllers;
using OrderAPI.DataAccess.Models;
using OrderAPI.DataTransfer;
using OrderAPI.DataAccess;
using OrderAPI.DataTransfer.Extentions;

namespace OrderAPI.Tests.UnitTests
{
    public class OrderControllerUnitTestsTest
    {
        private DbContextOptions<DbAccess> _options;
        private OrdersController _controller;
        private ILogger<OrdersController> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _options = new DbContextOptionsBuilder<DbAccess>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _mockLogger = new LoggerFactory().CreateLogger<OrdersController>();
            _controller = new OrdersController(new DbAccess(_options), _mockLogger);
        }

        [TearDown]
        public async Task TearDown()
        {
            using (var context = new DbAccess(_options))
            {
                context.Orders.RemoveRange(context.Orders);
                await context.SaveChangesAsync();
            }
        }

        [Test]
        public async Task CreateOrder_ValidModel_ReturnsCreated()
        {
            // Arrange
            var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "Testname", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            var createdResult = result as ObjectResult;
            Assert.That(createdResult?.StatusCode, Is.EqualTo(201));
        }

        [Test]
        public async Task CreateOrder_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("OrderNumber", "Required");
            var orderRequest = new OrderRequest();

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            Assert.That((result as BadRequestObjectResult)?.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task CreateOrder_DuplicateOrder_ReturnsConflict()
        {
            // Arrange
            var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "Test", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };

            using (var context = new DbAccess(_options))
            {
                var existingOrder = new Order { OrderNumber = 1, CustomerName = "Existing Customer", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() };
                await context.Orders.AddAsync(existingOrder);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
            Assert.That((result as ConflictObjectResult)?.StatusCode, Is.EqualTo(409));
        }

        [Test]
        public async Task GetAllOrders_ReturnsOk()
        {
            // Arrange
            using (var context = new DbAccess(_options))
            {
                var orders = new List<Order>
                {
                    new Order { OrderNumber = 1, CustomerName = "Customer1", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() },
                    new Order { OrderNumber = 2, CustomerName = "Customer2", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() }
                };
                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.GetAllOrders();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));

            var ordersResponse = okResult?.Value as IEnumerable<OrderResponse>;
            Assert.That(ordersResponse, Is.Not.Null);
            Assert.That(ordersResponse.Count(), Is.EqualTo(2));

            var order1 = ordersResponse?.FirstOrDefault(o => o.OrderNumber == 1);
            var order2 = ordersResponse?.FirstOrDefault(o => o.OrderNumber == 2);
            
            Assert.That(order1, Is.Not.Null);
            Assert.That(order1?.CustomerName, Is.EqualTo("Customer1"));
            
            Assert.That(order2, Is.Not.Null);
            Assert.That(order2?.CustomerName, Is.EqualTo("Customer2"));
        }

        [Test]
        public async Task GetOrderById_ValidId_ReturnsOk()
        {
            // Arrange
            var order = new Order { OrderNumber = 1, CustomerName = "Customer1", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() };
            using (var context = new DbAccess(_options))
            {
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.GetOrderById(1);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult?.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetOrderById_InvalidId_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetOrderById(999);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task UpdatePaymentStatus_ValidOrder_ReturnsOk_Paid()
        {
            // Arrange
            var order = new Order { OrderNumber = 1, CustomerName = "Customer1", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() };
            using (var context = new DbAccess(_options))
            {
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.UpdatePaymentStatus(1, true);
            using (var context = new DbAccess(_options))
            {
                var updatedOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == 1);

                // Assert
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                Assert.That(updatedOrder?.Status, Is.EqualTo("Paid"));
                Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task UpdatePaymentStatus_ValidOrder_ReturnsOk_Cancelled()
        {
            // Arrange
            var order = new Order { OrderNumber = 1, CustomerName = "Customer1", OrderDate = DateTime.Now, Status = "New", OrderItems = new List<OrderItem>() };
            using (var context = new DbAccess(_options))
            {
                await context.Orders.AddAsync(order);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.UpdatePaymentStatus(1, false);
            using (var context = new DbAccess(_options))
            {
                var updatedOrder = await context.Orders.FirstOrDefaultAsync(o => o.OrderNumber == 1);

                // Assert
                Assert.That(result, Is.InstanceOf<OkObjectResult>());
                var okResult = result as OkObjectResult;
                Assert.That(updatedOrder?.Status, Is.EqualTo("Cancalled"));
                Assert.That(okResult?.StatusCode, Is.EqualTo(200));
            }
        }

        [Test]
        public async Task UpdatePaymentStatus_InvalidOrder_ReturnsNotFound()
        {
            // Act
            var result = await _controller.UpdatePaymentStatus(999, true);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
            var notFoundResult = result as NotFoundObjectResult;
            Assert.That(notFoundResult?.StatusCode, Is.EqualTo(404));
        }
    }
}