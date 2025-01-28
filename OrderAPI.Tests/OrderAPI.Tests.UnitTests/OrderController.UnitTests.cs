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

namespace OrderAPI.Tests.UnitTests
{
    public class OrdersControllerTests
    {
        private Mock<DbAccess> _mockDbContext;
        private Mock<ILogger<OrdersController>> _mockLogger;
        private OrdersController _controller;

        [SetUp]
        public void Setup()
        {
            _mockDbContext = new Mock<DbAccess>(new DbContextOptions<DbAccess>());
            _mockLogger = new Mock<ILogger<OrdersController>>();
            _controller = new OrdersController(_mockDbContext.Object, _mockLogger.Object);
        }
        
        // CreateOrder Tests
        [Test]
        public async Task CreateOrder_ValidModel_ReturnsCreated()
        {
            // Arrange
            var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "Testname", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };

            // Mock the DbContext behavior
            _mockDbContext.Setup(db => db.Orders.Add(It.IsAny<Order>()));
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            var createdResult = result as CreatedResult;

            // Check that the result is of type CreatedResult
            Assert.That(createdResult?.Value, Is.EqualTo($"Order {orderRequest.OrderNumber} successfully created."));
            Assert.That(createdResult?.StatusCode, Is.EqualTo(201));

            // Verify that SaveChangesAsync and Add were called
            _mockDbContext.Verify(db => db.Orders.Add(It.IsAny<Order>()), Times.Once);
            _mockDbContext.Verify(db => db.SaveChangesAsync(default), Times.Once);
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
                var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "John Doe", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };

                // Mock the behavior for FirstOrDefault to return a duplicate order
                var mockOrder = new Order { OrderNumber = 1 }; // Assume this matches the order number
                _mockDbContext.Setup(db => db.Orders.FirstOrDefault(It.IsAny<Func<Order, bool>>()))
                            .Returns(mockOrder);

                // Act
                var result = await _controller.CreateOrder(orderRequest);

                // Assert
                Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
                Assert.That((result as ConflictObjectResult)?.StatusCode, Is.EqualTo(409));
            }

        [Test]
        public async Task CreateOrder_DbUpdateException_ReturnsInternalServerError()
        {
            // Arrange
            var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "Testname", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            Assert.That((result as ObjectResult)?.StatusCode, Is.EqualTo(500));
        }

        [Test]
        public async Task CreateOrder_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var orderRequest = new OrderRequest { OrderNumber = 1, CustomerName = "Testname", OrderDate = DateTime.Now, OrderItems = new List<OrderItemRequest>() };
            _mockDbContext.Setup(db => db.SaveChangesAsync(default)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.CreateOrder(orderRequest);

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            Assert.That((result as ObjectResult)?.StatusCode, Is.EqualTo(500));
        }
    }
}
